using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
/*
namespace Basdidon.Dialogue
{
    public abstract class DialogueCharacterBase : ScriptableObject
    {
        DialogueDatabase DialogueDatabase;
        [field:SerializeField] public string Name { get; private set; }
        public abstract Sprite Sprite { get; }

        public void Initialize(DialogueDatabase dialogueDatabase)
        {
            DialogueDatabase = dialogueDatabase;
            DialogueDatabase.AddCharacter(this);
            AssetDatabase.AddObjectToAsset(this, dialogueDatabase);
            SaveChanges();
        }

        private void OnDestroy()
        {
            Debug.Log("destroy");
            DialogueDatabase.RemoveCharacter(this);
        }

        private void OnValidate()
        {
            name = Name;
            SaveChanges();
        }

        private void SaveChanges()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }
    }

    [CreateAssetMenu(menuName = "Dialogues/Database")]
    public class DialogueDatabase : ScriptableObject
    {
        [field: SerializeField] public List<DialogueCharacterBase> Characters { get; private set; } = new();

        internal void CreateDialogueCharacter(Type type)
        {
            Debug.Log(type);
            var dialogueCharacter = CreateInstance(type) as DialogueCharacterBase;
            //var path = AssetDatabase.GenerateUniqueAssetPath($"Assets/DialogueDatabase/{type.Name}.asset");
            //ProjectWindowUtil.CreateAsset(dialogueCharacter,path);

            dialogueCharacter.Initialize(this);
        }

        internal void AddCharacter(DialogueCharacterBase character)
        {
            Characters.Add(character);
        }

        internal void RemoveCharacter(DialogueCharacterBase character)
        {
            Characters.Remove(character);
        }
    }

    [CustomEditor(typeof(DialogueDatabase))]
    public class DialogueDatabaseEditor: Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();

            if (target is DialogueDatabase dialogueDatabase)
            {
                var derivedTypes = GetSubClassOf(typeof(DialogueCharacterBase));

                if (derivedTypes.Any())
                {
                    List<string> derivedTypesName = derivedTypes.Select(t => t.Name).ToList();
                    var typesDropdown = new DropdownField("type",derivedTypesName,0);

                    var createDialogueCharacterBtn = new Button() { text = "Create Dialogue Character" };
                    createDialogueCharacterBtn.clicked += () =>
                    {
                        var type = derivedTypes.ElementAtOrDefault(typesDropdown.index);
                        dialogueDatabase.CreateDialogueCharacter(type);
                    };
                    container.Add(typesDropdown);
                    container.Add(createDialogueCharacterBtn);

                    var items = dialogueDatabase.Characters;
                    // list
                    var defaultCharacterList = new PropertyField(serializedObject.FindProperty("<Characters>k__BackingField"));
                    var characterList = new ListView(
                        items,
                        16,
                        () => {
                            var objField = new ObjectField();
                            objField.SetEnabled(false);
                            return objField;
                        },
                        (e, i) => (e as ObjectField).value = items[i]
                    )
                    {
                        showBorder = true,
                        showFoldoutHeader = true,
                        headerTitle = "Characters",
                        showBoundCollectionSize = false,
                        bindingPath = "<Characters>k__BackingField",
                    };

                    
                    //characterListview.showAddRemoveFooter = false;
                    container.Add(defaultCharacterList);
                    container.Add(characterList);
                }
                else
                {
                    var warningLabel = new Label() { text = "need to prepared"};
                    container.Add(warningLabel);
                }
            }
            return container;
        }
        
        public static IEnumerable<Type> GetSubClassOf(Type type)
        {
            // Get all types in the assembly
            var assembly = Assembly.GetExecutingAssembly();
            var allTypes = assembly.GetTypes();

            // Find types that are subclasses of type
            var derivedTypes = allTypes.Where(t => t.IsSubclassOf(type));

            return derivedTypes;
        }
    }
}

*/