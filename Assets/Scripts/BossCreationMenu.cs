using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class BossCreationMenu: EditorWindow {

	BossParametersAsset cloneSource;
	BossParametersAsset currentAsset;
	SerializedObject currentAssetSerializedObject;
	SerializedProperty currentAssetParametersProperty;
	
	private const string rootFolderName = "Bosses";
	private const string rootFolderPath = "Assets/" + rootFolderName;
	private static readonly string[] subfolders = new string[] {
		"Materials",
		"Textures",
		"Animations",
		"Models",
		"Prefabs"
	};


	[MenuItem("Asset Management/Boss Editor")]
	static void Init () {
		BossCreationMenu menu = (BossCreationMenu)EditorWindow.GetWindow(typeof(BossCreationMenu));
		menu.Show();
	}

	bool isDeleting = false;
	void OnGUI () {

		GUILayout.Space(10);
		if (GUILayout.Button("Create New Boss Asset")) {
			CreateNewAsset();
		}
		GUILayout.Space(10);

		//Edit the asset currently populated in the selected asset object field	
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Selected Boss Asset");
		BossParametersAsset asset = EditorGUILayout.ObjectField(currentAsset, typeof(BossParametersAsset), true) as BossParametersAsset;
		EditAsset(asset);
		EditorGUILayout.EndHorizontal();
		
		if (currentAsset != null) {

			EditorGUILayout.PropertyField(currentAssetParametersProperty);
			currentAssetSerializedObject.ApplyModifiedProperties();
			currentAsset.name = currentAsset.bossParameters.name;
			
			if (!isDeleting) {

				GUILayout.BeginHorizontal();

				// asset clone button. we only present this option if the asset actually exists in the assets folder 
				if (AssetDatabase.GetAssetPath(currentAsset).Length > 0) {
					if (GUILayout.Button("Make Clone")) {
						CloneAsset();
					}

				//otherwise, we draw the create asset button, which creates new folders for this asset
				} else {
					if (GUILayout.Button("Create Asset")) {
						SaveAsset();
					}
				}

				//handle deleting boss. we don't want to delete immediately, so set isDeleting to true
				if (GUILayout.Button("Delete")) {
					isDeleting = true;
				}

				GUILayout.EndHorizontal();

			//handle isDeleting state
			} else {
				
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();

				GUILayout.Label("Are you sure?");
				
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();

				Color color = GUI.color;
				GUI.backgroundColor = Color.red;
				if (GUILayout.Button("Delete")) {
					DeleteAsset();
					isDeleting = false;
				}
				GUI.backgroundColor = color;
				if (GUILayout.Button ("Cancel")) {
					isDeleting = false;
				}
				
				GUILayout.EndHorizontal();
			}
		}
	}
	void EditAsset (BossParametersAsset asset) {
		if (asset != null) {
			if (currentAsset != asset || currentAssetParametersProperty == null) {
				currentAssetSerializedObject = new SerializedObject(asset);
				currentAssetParametersProperty = currentAssetSerializedObject.FindProperty("bossParameters");			
			} 
		} else {
			currentAssetSerializedObject = null;
			currentAssetParametersProperty = null;
		}
		currentAsset = asset;
	}
	void CreateNewAsset () {
		BossParametersAsset asset = ScriptableObject.CreateInstance<BossParametersAsset>();
		asset.bossParameters = DefaultBossParameters();
		asset.name = asset.bossParameters.name;
		EditAsset(asset);
	}

	void CloneAsset () {
		cloneSource = currentAsset;
		BossParametersAsset clone = Object.Instantiate(currentAsset);
		clone.name = currentAsset.name + " Clone";
		clone.bossParameters.name += " Clone";
		EditAsset(clone);
	}
	
	void DeleteAsset () {
		string parentDirectory = Path.GetDirectoryName(AssetDatabase.GetAssetPath(currentAsset));
		currentAsset = null;
		EditAsset(currentAsset);
		FileUtil.DeleteFileOrDirectory(parentDirectory);
		Debug.Log(parentDirectory);
		AssetDatabase.Refresh();
	}

	void SaveAsset() {

		//create boss root path if it doesn't exist
		if (!AssetDatabase.IsValidFolder(rootFolderPath)) {
			AssetDatabase.CreateFolder("Assets", rootFolderName);
		}

		//create the boss folder. if it exists already, return with an error notification
		string bossName = currentAsset.bossParameters.name;
		string bossPath = rootFolderPath + "/" + bossName;
		if (AssetDatabase.IsValidFolder(bossPath)) {
			ShowNotification(new GUIContent("Error: Assets for boss " + bossName + " already exist!"));
			return;
		}
		AssetDatabase.CreateFolder(rootFolderPath, bossName);

		//create the boss asset 
		string assetPath = bossPath + "/" + bossName + ".asset";
		AssetDatabase.CreateAsset(currentAsset, assetPath);
		AssetDatabase.SaveAssets();

		//create subfolders. if we have any cloned subfolder paths we are supposed to copy,
		//copy them instead of creating new subfolders
		if(cloneSource != null) {
			CloneSubfolders(cloneSource, bossPath);
		} else {
			CreateNewSubfolders(bossPath);
		}
		AssetDatabase.Refresh();
	}

	void CloneSubfolders (BossParametersAsset source, string path) {
		string parentDirectory = Path.GetDirectoryName(AssetDatabase.GetAssetPath(cloneSource));
		string[] parentDirectoryContents = AssetDatabase.GetSubFolders(parentDirectory);

		// iterate through the subfolders array and copy any matching folders to the new boss asset folder
		for (int i = 0; i < subfolders.Length; i++) {
			bool clonedSubfolder = false;
			for (int j = 0; j < parentDirectoryContents.Length; j++) {
				string itemName = new DirectoryInfo(parentDirectoryContents[j]).Name;
				if (itemName == subfolders[i]) {
					FileUtil.CopyFileOrDirectory(parentDirectoryContents[j], path + "/" + subfolders[i]);
					clonedSubfolder = true;
				}
			}
			//make sure to fill any folders that were missing!
			if (!clonedSubfolder) {
				AssetDatabase.CreateFolder(path, subfolders[i]);
			}
		}
	}

	void CreateNewSubfolders (string path) {
		for (int i = 0; i < subfolders.Length; i++) {
			AssetDatabase.CreateFolder(path, subfolders[i]);	
		}
	}

	static BossParameters DefaultBossParameters () {
		BossParameters parameters = new BossParameters();
		parameters.name = GetDefaultBossName();
		parameters.subtitle = "NO REFUGE";
		parameters.strongType = Element.Fire;
		parameters.weakType = Element.Water;
		parameters.healthPoints = 100;
		parameters.attackDelayTime = new LimitedRange(0f, 0f, 10f, 10f);
		parameters.moveDelayTime = new LimitedRange(0f, 0f, 10f, 10f);
		parameters.description = "Enter a description here";
		return parameters;
	}

	static readonly string[] bossNames = new string[] {
		"Psycho Mantis",
		"GLaDOS",
		"Doctor Robotnik",
		"Bowser",
		"Ridley",
		"Mike Tyson",
		"Lavos",
		"Big Daddy",
		"Master Hand",
		"Ganon",
		"Giygas",
		"Eve"
	};
	static string GetDefaultBossName () {
		int idx = Random.Range(0, bossNames.Length);
		return bossNames[idx];
	}
}

