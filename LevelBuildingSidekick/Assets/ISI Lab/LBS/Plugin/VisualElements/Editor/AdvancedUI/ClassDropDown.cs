using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine.UIElements;
using ISILab.Commons.Utility;

namespace ISILab.LBS.VisualElements
{
    [UxmlElement]
    public partial class ClassDropDown : DropdownField
    {
   //     public new class UxmlFactory : UxmlFactory<ClassDropDown, UxmlTraits> {}

        private readonly UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription { name = "Label", defaultValue = "Class DropDown" };
        
        //public void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        //{
        //    UnityEngine.Debug.Log("Class Dropdown Init");
        //    //Init(ve, bag, cc); // QUE?
        //
        //    ClassDropDown field = (ClassDropDown)ve;
        //    field.Label = m_Label.GetValueFromBag(bag, cc);
        //    
        //}

        #region FIELDS
        //Label label;

        Type type;

        bool filterAbstract;
        private List<Type> types;

        #endregion

        public string Value
        {
            get => value;
            set
            {
                if (choices.Contains(value))
                    this.value = value;
            }
        }

        //public string Label
        //{
        //    get => label?.text;
        //    set 
        //    {
        //        var notify = (INotifyValueChanged<string>)label;
        //        UnityEngine.Assertions.Assert.IsNotNull(notify, "Cast fallo");
        //        notify.SetValueWithoutNotify(value);
        //    }
        //}

        public Type TypeValue => types[choices.IndexOf(value)];

        public Type Type
        {
            get => type;
            set
            {
                type = value;
                UpdateOptions();
            }
        }

        public bool FilterAbstract
        {
            get => filterAbstract;
            set
            {
                filterAbstract = value;
                UpdateOptions();
            }
        }

        public ClassDropDown()
        {
            
        }

        public void Init()
        {
            //label = this.Q<Label>();
            //UnityEngine.Assertions.Assert.IsNotNull(label, "No se encontro el visual element");
            //Label = "Class DropDown";

            label = "Class DropDown";
            this.SetValueWithoutNotify("");
        }

        void UpdateOptions()
        {
            choices.Clear();

            List<Type> types = null;

            if (Type.IsClass)
            {
                types = Reflection.GetAllSubClassOf(Type).ToList();
            }
            else if (Type.IsInterface)
            {
                types = Reflection.GetAllImplementationsOf(Type).ToList();
            }

            if (filterAbstract)
            {
                types = types.Where(t => !t.IsAbstract).ToList();
            }

            var options = types.Select(t => t.Name).ToList();

            this.types = types;
            choices = options;
        }

        public object GetChoiceInstance()
        {
            object obj = null;
            var dv = value;
            var dx = choices.IndexOf(dv);

            if (dx < 0)
                return null;

            var t = types[dx];
            try
            {
                obj = Activator.CreateInstance(t);
            }
            catch
            {
                throw new FormatException(t + " class needs to have an empty constructor.");
            }

            return obj;
        }

    }
}