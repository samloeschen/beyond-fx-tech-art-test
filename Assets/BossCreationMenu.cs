using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class BossCreationMenu: EditorWindow {

	BossParametersAsset cloneSourceAsset;
	BossParametersAsset cloneAsset;
	SerializedObject cloneAssetSerializedObject;
	SerializedProperty cloneAssetParametersProperty;
	bool shouldCopySubfolders = true;


	BossParametersAsset newAsset;
	SerializedObject newAssetSerializedObject; 
	SerializedProperty newAssetParametersProperty;

	[MenuItem("Asset Creation/Create Boss")]
	static void Init () {
		BossCreationMenu menu = (BossCreationMenu)EditorWindow.GetWindow(typeof(BossCreationMenu));
		menu.Show();

	}
	void OnGUI () {

		// Always have a new asset in memory that we can edit. This asset doesn't exist on disk yet, it's just our working copy.
		// Create a serialized object corresponding to the asset, and get the property for boss parameters. 
		if (newAsset == null || newAssetParametersProperty == null) {
			newAsset = ScriptableObject.CreateInstance<BossParametersAsset>();
			newAsset.bossParameters = DefaultBossParameters();
			newAssetSerializedObject = new SerializedObject(newAsset);
			newAssetParametersProperty = newAssetSerializedObject.FindProperty("bossParameters");
		}

		//Draw the new asset editor
		GUILayout.Space(10);
		EditorGUILayout.LabelField("Create a new boss asset: ", EditorStyles.largeLabel);
		GUILayout.Space(5);

		//Draw Property Drawer for the new asset's BossParameters
		EditorGUILayout.PropertyField(newAssetParametersProperty);
		newAssetSerializedObject.ApplyModifiedProperties();

		//Draw Create button
		if (GUILayout.Button("Create")) {
			CreateNewBoss();
		}

		//Draw the cloned asset editor
		GUILayout.Space(20);
		EditorGUILayout.LabelField("Clone a preexisting boss asset: ", EditorStyles.largeLabel);
		GUILayout.Space(5);
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Asset to clone from:");

		BossParametersAsset asset = EditorGUILayout.ObjectField(cloneSourceAsset, typeof(BossParametersAsset), true) as BossParametersAsset;
		if (asset != cloneSourceAsset){
			if (asset != null) {
				cloneAsset = Object.Instantiate(asset);
				cloneAssetSerializedObject = new SerializedObject(cloneAsset);
				cloneAssetParametersProperty = cloneAssetSerializedObject.FindProperty("bossParameters");		
			} else {
				cloneAssetParametersProperty = null;
				cloneAsset = null;
			}
		}
		cloneSourceAsset = asset;

		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Clone Boss Subfolders?");
		shouldCopySubfolders = EditorGUILayout.Toggle(shouldCopySubfolders);
		EditorGUILayout.EndHorizontal();

		if(cloneAssetParametersProperty != null) {
			EditorGUILayout.PropertyField(cloneAssetParametersProperty);
			cloneAssetSerializedObject.ApplyModifiedProperties();
		}

		if (GUILayout.Button("Clone")) {
			CloneBoss();
		}
	}

	static BossParameters DefaultBossParameters () {
		BossParameters parameters = new BossParameters();
		parameters.name = "Psycho Mantis";
		parameters.subtitle = "NO REFUGE";
		parameters.strongType = Element.Fire;
		parameters.weakType = Element.Water;
		parameters.healthPoints = 100;
		parameters.attackDelayTime = new LimitedRange(0f, 0f, 10f, 10f);
		parameters.moveDelayTime = new LimitedRange(0f, 0f, 10f, 10f);
		parameters.description = "Enter a description here";
		return parameters;
	}

	private const string rootFolderName = "Bosses";
	private const string rootFolderPath = "Assets/" + rootFolderName;
	private static readonly string[] subfolders = new string[] {
		"Materials",
		"Textures",
		"Animations",
		"Models",
		"Prefabs"
	};

	void CreateNewBoss () {

		var bossName = newAsset.bossParameters.name;
		if (!AssetDatabase.IsValidFolder(rootFolderPath)) {
			AssetDatabase.CreateFolder("Assets", rootFolderName);
		}

		//create boss path
		string bossPath = MakeBossDirectory(bossName);
		if (bossPath == null) { return; }

		// create our scriptable object
		SaveBossParametersAsset(newAsset, bossPath, bossName);

		//create subfolders
		for (int i = 0; i < subfolders.Length; i++) {
			AssetDatabase.CreateFolder(bossPath, subfolders[i]);
		}
	}

	void CloneBoss () {
		
		string bossName = cloneAsset.bossParameters.name;
		string bossPath = MakeBossDirectory(bossName);
		if (bossPath == null) { return; }

		SaveBossParametersAsset(cloneAsset, bossPath, bossName);

		if (shouldCopySubfolders) {

			//get parent directory of bossToCopy
			string parentDirectory = Path.GetDirectoryName(AssetDatabase.GetAssetPath(cloneAsset));
			string[] parentDirectoryContents = AssetDatabase.GetSubFolders(parentDirectory);

			// iterate through the subfolders array and copy any matching folders to the new boss asset folder
			for (int i = 0; i < subfolders.Length; i++) {
				bool foundMatch = false;
				for (int j = 0; j < parentDirectoryContents.Length; j++) {
					string itemName = new DirectoryInfo(parentDirectoryContents[j]).Name;
					if (itemName == subfolders[i]) {
						FileUtil.CopyFileOrDirectory(parentDirectoryContents[j], bossPath + "/" + subfolders[i]);
						foundMatch = true;
					}
				}

				//make sure to fill any folders that were missing!
				if (!foundMatch) {
					AssetDatabase.CreateFolder(bossPath, subfolders[i]);
				}
				AssetDatabase.Refresh();
			}
		}
	}

	string MakeBossDirectory (string bossName) {
		// make sure a boss directory for this name doesn't already exist
		string bossPath = rootFolderPath + "/" + bossName;
		if (AssetDatabase.IsValidFolder(bossPath)) {
			ShowNotification(new GUIContent("Error: Assets for boss " + bossName + " already exist!"));
			return null;
		}
		AssetDatabase.CreateFolder(rootFolderPath, bossName);
		return bossPath;
	}

	private void SaveBossParametersAsset (BossParametersAsset asset, string path, string bossName) {
		string assetPath = path + "/" + bossName + ".asset";
		AssetDatabase.CreateAsset(asset, assetPath);
		AssetDatabase.SaveAssets();
	}

}

