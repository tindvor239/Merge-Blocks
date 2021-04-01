using UnityEngine.UI;
using System;
using System.Collections.Generic;
namespace UnityEngine.CustomComponents
{
    public class CustomUIComponent : MonoBehaviour
    {
        [Serializable]
        public class Display : CustomDictionary<string, MaskableGraphic> { };
        [SerializeField]
        protected Display[] displays;
        [Serializable]
        public class UserInterface : CustomDictionary<string, Selectable> { };
        [SerializeField]
        protected UserInterface[] userInterfaces;
        #region Properties
        public Display[] Displays { get => displays; }
        public UserInterface[] UserInterfaces { get => userInterfaces; }
        #endregion
        protected virtual void Start()
        {
            
        }
        protected virtual void Update()
        {
            
        }
        protected virtual void FixedUpdate()
        {
            
        }

        public MaskableGraphic GetDisplay(string name)
        {
            foreach (Display userinterface in displays)
            {
                if (name == userinterface.Key)
                {
                    return userinterface.Value;
                }
            }
            return null;
        }

        public Selectable GetSelectable(string name)
        {
            foreach(UserInterface userinterface in userInterfaces)
            {
                if(name == userinterface.Key)
                {
                    return userinterface.Value;
                }
            }
            return null;
        }
    }
}
