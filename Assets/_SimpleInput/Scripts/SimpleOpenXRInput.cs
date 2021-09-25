using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

namespace SimpleInput
{
    /// <summary>Provides functionality for basic OpenXR Functions.</summary>
    /// <remarks>
    /// Uses callbacks and static methods for a simple high level OpenXR Input manager.
    /// Check BasicInputTest.cs for a better understanding on how to use.
    /// </remarks>
    public class SimpleOpenXRInput : MonoBehaviour
    {
        #region -- Public Interface ---------------------------------

        /// <summary> Call to make controllers vibrate / rumble / etc...
        /// </summary>
        /// <param name="controllerID">Controller to Vibrate</param>
        /// <param name="amplitude">Vibrate Amount (0.0f - 1.0f)</param>
        /// <param name="duration">Time in seconds vibration will happen for</param>
        public static void PlayHaptics(int controllerID, float amplitude, float duration)
        {
            Haptics.PlayHaptics(controllerID, amplitude, duration);
        }

        /// <summary> Invoked anytime joystick position is updated -- JoystickPositionUpdate(int controllerID, Vector2 axis) 
        /// </summary>
        public static event AxisUpdated onJoystickUpdate;

        /// <summary> Invoked anytime Grip is updated -- GripPullUpdate(int controllerID, float value)
        /// </summary>
        public static event FloatUpdated onGripPullUpdate;

        /// <summary> Invoked anytime Trigger is updated -- TriggerPullUpdate(int controllerID, float value)
        /// </summary>
        public static event FloatUpdated onTriggerPullUpdate;

        /// <summary> Invoked when Joystick is clicked (pressed in) -- JoystickClickUpdate(int controllerID)
        /// </summary>
        public static event ButtonUpdated onJoystickClicked;

        /// <summary> Invoked when Trigger is clicked -- TriggerClickUpdate(int controllerID)
        /// </summary>
        public static event ButtonUpdated onTriggerClicked;

        /// <summary> Invoked when Primary Button is pressed -- PrimaryButtonUpdate(int controllerID)
        /// </summary>
        public static event ButtonUpdated onPrimaryButtonUpdate;

        /// <summary> Invoked when Secondary Button is pressed -- SecondaryButtonUpdate(int controllerID)
        /// </summary>
        public static event ButtonUpdated onSecondaryButtonUpdate;

        /// <summary> Invoked when Menu Button is pressed -- MenuButtonUpdate(int controllerID)
        /// </summary>
        public static event ButtonUpdated onMenuButtonUpdate;

        #endregion

        #region -- Mono --------------------------------------------

        // Can be initialized elsewhere if you don't want to use the Awake Method.
        void Awake()
        {
            SetupOpenXRInput();
        }

        #endregion

        #region -- Internal ----------------------------------------

        #region Delegates and Variables

        public delegate void AxisUpdated(int controllerID, Vector2 axis);
        public delegate void FloatUpdated(int controllerID, float value);
        public delegate void ButtonUpdated(int controllerID);
        Haptics myHaptics;

        #endregion

        #region Action Map

        //Build Action Map based on Unity OpenXR standard for binding configurations
        void SetupOpenXRInput()
        {
            List<InputAction> rightHandInputActions = new List<InputAction>();
            List<InputAction> leftHandHandInputActions = new List<InputAction>();
            InputActionMap actionMap = new InputActionMap();

            //Setup Right Hand Actions And Add To Map
            rightHandInputActions.Add(actionMap.AddAction("Joystick_Right", binding: "<XRController>{RightHand}/{primary2DAxis}"));
            rightHandInputActions.Add(actionMap.AddAction("JoystickClick_Right", binding: "<XRController>{RightHand}/{primary2DAxisClick}"));
            rightHandInputActions.Add(actionMap.AddAction("Trigger_Right", binding: "<XRController>{RightHand}/{trigger}"));
            rightHandInputActions.Add(actionMap.AddAction("TriggerClick_Right", binding: "<XRController>{RightHand}/{triggerButton}"));
            rightHandInputActions.Add(actionMap.AddAction("Grip_Right", binding: "<XRController>{RightHand}/{grip}"));
            rightHandInputActions.Add(actionMap.AddAction("Primary_Right", binding: "<XRController>{RightHand}/{primaryButton}"));
            rightHandInputActions.Add(actionMap.AddAction("Secondary_Right", binding: "<XRController>{RightHand}/{secondaryButton}"));
            rightHandInputActions.Add(actionMap.AddAction("Menu_Right", binding: "<XRController>{RightHand}/menu"));

            //Setup Left Hand Actions And Add To Map
            leftHandHandInputActions.Add(actionMap.AddAction("Joystick_Left", binding: "<XRController>{LeftHand}/{primary2DAxis}"));
            leftHandHandInputActions.Add(actionMap.AddAction("JoystickClick_Left", binding: "<XRController>{LeftHand}/{primary2DAxisClick}"));
            leftHandHandInputActions.Add(actionMap.AddAction("Trigger_Left", binding: "<XRController>{LeftHand}/{trigger}"));
            leftHandHandInputActions.Add(actionMap.AddAction("TriggerClick_Left", binding: "<XRController>{LeftHand}/{triggerButton}"));
            leftHandHandInputActions.Add(actionMap.AddAction("Grip_Left", binding: "<XRController>{LeftHand}/{grip}"));
            leftHandHandInputActions.Add(actionMap.AddAction("Primary_Left", binding: "<XRController>{LeftHand}/{primaryButton}"));
            leftHandHandInputActions.Add(actionMap.AddAction("Secondary_Left", binding: "<XRController>{LeftHand}/{secondaryButton}"));
            leftHandHandInputActions.Add(actionMap.AddAction("Menu_Left", binding: "<XRController>{LeftHand}/menu"));

            //IMPORTANT: Make sure you enable the Map once created.
            actionMap.Enable();

            myHaptics = new Haptics(rightHandInputActions.ToArray(), leftHandHandInputActions.ToArray());
            new InputActions(this, actionMap);
        }

        #endregion

        #region Callbacks

        //Joystick
        internal void JoystickPositionUpdate(int controllerID, Vector2 axis)
        {
            onJoystickUpdate?.Invoke(controllerID, axis);
        }

        internal void JoystickClickUpdate(int controllerID)
        {
            onJoystickClicked?.Invoke(controllerID);
        }

        //Trigger
        internal void TriggerPullUpdate(int controllerID, float value)
        {
            onTriggerPullUpdate?.Invoke(controllerID, value);
        }

        internal void TriggerClickUpdate(int controllerID)
        {
            onTriggerClicked?.Invoke(controllerID);
        }

        //Grip
        internal void GripPullUpdate(int controllerID, float value)
        {
            onGripPullUpdate?.Invoke(controllerID, value);
        }

        //Primary, Secondary and Menu
        internal void PrimaryButtonUpdate(int controllerID)
        {
            onPrimaryButtonUpdate?.Invoke(controllerID);
        }

        internal void SecondaryButtonUpdate(int controllerID)
        {
            onSecondaryButtonUpdate?.Invoke(controllerID);
        }

        internal void MenuButtonUpdate(int controllerID)
        {
            onMenuButtonUpdate?.Invoke(controllerID);
        }

        #endregion

        #region Helper Classes

        #region Input Actions

        /// <summary> Class used for converting all the Input actions
        /// into events.
        /// </summary>
        class InputActions
        {
            SimpleOpenXRInput myInput;

            public InputActions(SimpleOpenXRInput input, InputActionMap actionMap)
            {
                myInput = input;

                //Joystick
                InputAction rightStickAction = actionMap.FindAction("Joystick_Right");
                if (rightStickAction != null) rightStickAction.performed += Joystick_Right;

                InputAction leftStickAction = actionMap.FindAction("Joystick_Left");
                if (leftStickAction != null) leftStickAction.performed += Joystick_Left;

                InputAction rightStickClick = actionMap.FindAction("JoystickClick_Right");
                if (rightStickClick != null) rightStickClick.performed += JoystickClick_Right;

                InputAction leftStickClick = actionMap.FindAction("JoystickClick_Left");
                if (leftStickClick != null) leftStickClick.performed += JoystickClick_Left;

                //Trigger
                InputAction rightTriggerPull = actionMap.FindAction("Trigger_Right");
                if (rightTriggerPull != null) rightTriggerPull.performed += TriggerPull_Right;

                InputAction leftTriggerPull = actionMap.FindAction("Trigger_Left");
                if (leftTriggerPull != null) leftTriggerPull.performed += TriggerPull_Left;

                InputAction rightTriggerClick = actionMap.FindAction("TriggerClick_Right");
                if (rightTriggerClick != null) rightTriggerClick.performed += TriggerClick_Right;

                InputAction leftTriggerClick = actionMap.FindAction("TriggerClick_Left");
                if (leftTriggerClick != null) leftTriggerClick.performed += TriggerClick_Left;

                //Grip
                InputAction rightGripPull = actionMap.FindAction("Grip_Right");
                if (rightGripPull != null) rightGripPull.performed += Grip_Right;

                InputAction leftGripPull = actionMap.FindAction("Grip_Left");
                if (leftGripPull != null) leftGripPull.performed += Grip_Left;

                //Basic Buttons
                InputAction primaryRight = actionMap.FindAction("Primary_Right");
                if (primaryRight != null) primaryRight.performed += PrimaryButton_Right;

                InputAction primaryLeft = actionMap.FindAction("Primary_Left");
                if (primaryLeft != null) primaryLeft.performed += PrimaryButton_Left;

                InputAction secondaryRight = actionMap.FindAction("Secondary_Right");
                if (secondaryRight != null) secondaryRight.performed += SecondaryButton_Right;

                InputAction secondaryLeft = actionMap.FindAction("Secondary_Left");
                if (secondaryLeft != null) secondaryLeft.performed += SecondaryButton_Left;

                InputAction menuRight = actionMap.FindAction("Menu_Right");
                if (menuRight != null) menuRight.performed += MenuButton_Right;

                InputAction menuLeft = actionMap.FindAction("Menu_Left");
                if (menuLeft != null) menuLeft.performed += MenuButton_Left;
            }

            #region Joystick
            private void Joystick_Right(InputAction.CallbackContext obj)
            {
                Vector2 turnVector = obj.ReadValue<Vector2>();
                myInput.JoystickPositionUpdate(0, turnVector);
            }

            private void Joystick_Left(InputAction.CallbackContext obj)
            {
                Vector2 turnVector = obj.ReadValue<Vector2>();
                myInput.JoystickPositionUpdate(1, turnVector);
            }

            private void JoystickClick_Right(InputAction.CallbackContext obj)
            {
                myInput.JoystickClickUpdate(0);
            }

            private void JoystickClick_Left(InputAction.CallbackContext obj)
            {
                myInput.JoystickClickUpdate(1);
            }
            #endregion

            #region Trigger
            private void TriggerPull_Right(InputAction.CallbackContext obj)
            {
                float pullAmount = obj.ReadValue<float>();
                myInput.TriggerPullUpdate(0, pullAmount);
            }

            private void TriggerPull_Left(InputAction.CallbackContext obj)
            {
                float pullAmount = obj.ReadValue<float>();
                myInput.TriggerPullUpdate(1, pullAmount);
            }

            private void TriggerClick_Right(InputAction.CallbackContext obj)
            {
                myInput.TriggerClickUpdate(0);
            }

            private void TriggerClick_Left(InputAction.CallbackContext obj)
            {
                myInput.TriggerClickUpdate(1);
            }
            #endregion

            #region Grip
            private void Grip_Right(InputAction.CallbackContext obj)
            {
                float pullAmount = obj.ReadValue<float>();
                myInput.GripPullUpdate(0, pullAmount);
            }

            private void Grip_Left(InputAction.CallbackContext obj)
            {
                float pullAmount = obj.ReadValue<float>();
                myInput.GripPullUpdate(1, pullAmount);
            }
            #endregion

            #region Basic Buttons
            private void PrimaryButton_Right(InputAction.CallbackContext obj)
            {
                myInput.PrimaryButtonUpdate(0);
            }

            private void PrimaryButton_Left(InputAction.CallbackContext obj)
            {
                myInput.PrimaryButtonUpdate(1);
            }

            private void SecondaryButton_Right(InputAction.CallbackContext obj)
            {
                myInput.SecondaryButtonUpdate(0);
            }

            private void SecondaryButton_Left(InputAction.CallbackContext obj)
            {
                myInput.SecondaryButtonUpdate(1);
            }

            private void MenuButton_Right(InputAction.CallbackContext obj)
            {
                myInput.MenuButtonUpdate(0);
            }

            private void MenuButton_Left(InputAction.CallbackContext obj)
            {
                myInput.MenuButtonUpdate(1);
            }
            #endregion
        }

        #endregion

        #region Haptics

        /// <summary> Simple class used to access haptic feedback.
        /// </summary>
        /// <remarks> A somewhat hacky solution until there is a better way to detect both "XRControllerWithRumble"
        /// </remarks>
        class Haptics
        {
            static XRControllerWithRumble[] xrControllers = new XRControllerWithRumble[2];
            InputAction[] rightHandActions, leftHandActions;
            bool[] controllersAssigned = new bool[2];

            //Initialized wtih all possible input actions so we can detect when a controller was used.
            public Haptics(InputAction[] newRightHandActions, InputAction[] newLeftHandActions)
            {
                //Debug.Log(" Setting Up New Haptics");
                rightHandActions = newRightHandActions;
                leftHandActions = newLeftHandActions;
                for (int i = 0; i < rightHandActions.Length; i++)
                {
                    rightHandActions[i].performed += SetupRightController;
                }

                for (int i = 0; i < leftHandActions.Length; i++)
                {
                    leftHandActions[i].performed += SetupLeftController;
                }
            }

            static internal void PlayHaptics(int controllerID, float amplitude, float duration)
            {
                if (xrControllers[controllerID] != null && amplitude > .05f)
                {
                    //Debug.Log("Playing Haptics: " + controllerID + ", " + amplitude + ", " + duration);
                    xrControllers[controllerID].SendImpulse(amplitude, duration);
                }
            }

            void SetupRightController(InputAction.CallbackContext obj)
            {
                //Debug.Log("Setup Conrtoller Called");
                if (!controllersAssigned[0]) controllersAssigned[0] = AssignController(obj.action, 0);
                if (controllersAssigned[0])
                {
                    for (int i = 0; i < rightHandActions.Length; i++)
                    {
                        rightHandActions[i].performed -= SetupRightController;
                    }
                }
            }

            void SetupLeftController(InputAction.CallbackContext obj)
            {
                if (!controllersAssigned[1]) controllersAssigned[1] = AssignController(obj.action, 1);
                if (controllersAssigned[1])
                {
                    for (int i = 0; i < leftHandActions.Length; i++)
                    {
                        leftHandActions[i].performed -= SetupRightController;
                    }
                }
            }

            bool AssignController(InputAction inputAction, int handID)
            {
                var control = inputAction.activeControl;
                if (null == control)
                    return false;
                else
                {
                    if (control.device is XRControllerWithRumble rumble)
                    {
                        //Debug.Log("Controller "+handID+" Setup");
                        xrControllers[handID] = rumble;
                        return true;
                    }
                }
                return false;
            }
        }

        #endregion
        #endregion
        #endregion
    }
}