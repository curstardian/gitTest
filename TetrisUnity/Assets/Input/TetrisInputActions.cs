// Auto-generated equivalent of Unity Input System C# class generation.
// Matches Assets/Input/TetrisInputActions.inputactions
// Namespace: Tetris, Class: TetrisInputActions

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Tetris
{
    public partial class @TetrisInputActions : IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }

        public @TetrisInputActions()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""TetrisInputActions"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""a1b2c3d4-0001-0001-0001-000000000001"",
            ""actions"": [
                { ""name"": ""MoveLeft"",  ""type"": ""Button"", ""id"": ""a1b2c3d4-0001-0001-0001-000000000002"", ""expectedControlType"": ""Button"", ""processors"": """", ""interactions"": """", ""initialStateCheck"": false },
                { ""name"": ""MoveRight"", ""type"": ""Button"", ""id"": ""a1b2c3d4-0001-0001-0001-000000000003"", ""expectedControlType"": ""Button"", ""processors"": """", ""interactions"": """", ""initialStateCheck"": false },
                { ""name"": ""SoftDrop"",  ""type"": ""Button"", ""id"": ""a1b2c3d4-0001-0001-0001-000000000004"", ""expectedControlType"": ""Button"", ""processors"": """", ""interactions"": """", ""initialStateCheck"": false },
                { ""name"": ""HardDrop"",  ""type"": ""Button"", ""id"": ""a1b2c3d4-0001-0001-0001-000000000005"", ""expectedControlType"": ""Button"", ""processors"": """", ""interactions"": """", ""initialStateCheck"": false },
                { ""name"": ""RotateCW"",  ""type"": ""Button"", ""id"": ""a1b2c3d4-0001-0001-0001-000000000006"", ""expectedControlType"": ""Button"", ""processors"": """", ""interactions"": """", ""initialStateCheck"": false },
                { ""name"": ""RotateCCW"", ""type"": ""Button"", ""id"": ""a1b2c3d4-0001-0001-0001-000000000007"", ""expectedControlType"": ""Button"", ""processors"": """", ""interactions"": """", ""initialStateCheck"": false },
                { ""name"": ""Pause"",     ""type"": ""Button"", ""id"": ""a1b2c3d4-0001-0001-0001-000000000008"", ""expectedControlType"": ""Button"", ""processors"": """", ""interactions"": """", ""initialStateCheck"": false }
            ],
            ""bindings"": [
                { ""name"": """", ""id"": ""b1000001-0001-0001-0001-000000000001"", ""path"": ""<Keyboard>/leftArrow"",  ""interactions"": """", ""processors"": """", ""groups"": """", ""action"": ""MoveLeft"",  ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""b1000001-0001-0001-0001-000000000002"", ""path"": ""<Keyboard>/a"",          ""interactions"": """", ""processors"": """", ""groups"": """", ""action"": ""MoveLeft"",  ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""b1000001-0001-0001-0001-000000000003"", ""path"": ""<Keyboard>/rightArrow"", ""interactions"": """", ""processors"": """", ""groups"": """", ""action"": ""MoveRight"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""b1000001-0001-0001-0001-000000000004"", ""path"": ""<Keyboard>/d"",          ""interactions"": """", ""processors"": """", ""groups"": """", ""action"": ""MoveRight"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""b1000001-0001-0001-0001-000000000005"", ""path"": ""<Keyboard>/downArrow"",  ""interactions"": """", ""processors"": """", ""groups"": """", ""action"": ""SoftDrop"",  ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""b1000001-0001-0001-0001-000000000006"", ""path"": ""<Keyboard>/s"",          ""interactions"": """", ""processors"": """", ""groups"": """", ""action"": ""SoftDrop"",  ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""b1000001-0001-0001-0001-000000000007"", ""path"": ""<Keyboard>/space"",      ""interactions"": """", ""processors"": """", ""groups"": """", ""action"": ""HardDrop"",  ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""b1000001-0001-0001-0001-000000000008"", ""path"": ""<Keyboard>/upArrow"",    ""interactions"": """", ""processors"": """", ""groups"": """", ""action"": ""RotateCW"",  ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""b1000001-0001-0001-0001-000000000009"", ""path"": ""<Keyboard>/x"",          ""interactions"": """", ""processors"": """", ""groups"": """", ""action"": ""RotateCW"",  ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""b1000001-0001-0001-0001-000000000010"", ""path"": ""<Keyboard>/z"",          ""interactions"": """", ""processors"": """", ""groups"": """", ""action"": ""RotateCCW"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""b1000001-0001-0001-0001-000000000011"", ""path"": ""<Keyboard>/leftCtrl"",   ""interactions"": """", ""processors"": """", ""groups"": """", ""action"": ""RotateCCW"", ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""b1000001-0001-0001-0001-000000000012"", ""path"": ""<Keyboard>/p"",          ""interactions"": """", ""processors"": """", ""groups"": """", ""action"": ""Pause"",     ""isComposite"": false, ""isPartOfComposite"": false },
                { ""name"": """", ""id"": ""b1000001-0001-0001-0001-000000000013"", ""path"": ""<Keyboard>/escape"",     ""interactions"": """", ""processors"": """", ""groups"": """", ""action"": ""Pause"",     ""isComposite"": false, ""isPartOfComposite"": false }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Cache action map and individual actions
            m_Gameplay        = asset.FindActionMap("Gameplay", throwIfNotFound: true);
            m_Gameplay_MoveLeft  = m_Gameplay.FindAction("MoveLeft",  throwIfNotFound: true);
            m_Gameplay_MoveRight = m_Gameplay.FindAction("MoveRight", throwIfNotFound: true);
            m_Gameplay_SoftDrop  = m_Gameplay.FindAction("SoftDrop",  throwIfNotFound: true);
            m_Gameplay_HardDrop  = m_Gameplay.FindAction("HardDrop",  throwIfNotFound: true);
            m_Gameplay_RotateCW  = m_Gameplay.FindAction("RotateCW",  throwIfNotFound: true);
            m_Gameplay_RotateCCW = m_Gameplay.FindAction("RotateCCW", throwIfNotFound: true);
            m_Gameplay_Pause     = m_Gameplay.FindAction("Pause",     throwIfNotFound: true);
        }

        ~@TetrisInputActions()
        {
            Dispose();
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        // ── IInputActionCollection2 ───────────────────────────────────────

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action) => asset.Contains(action);

        public IEnumerator<InputAction> GetEnumerator() => asset.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Enable()  => asset.Enable();
        public void Disable() => asset.Disable();

        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
            => asset.FindAction(actionNameOrId, throwIfNotFound);

        public int FindBinding(InputBinding bindingMask, out InputAction action)
            => asset.FindBinding(bindingMask, out action);

        // ── Gameplay action map ───────────────────────────────────────────

        private readonly InputActionMap m_Gameplay;
        private readonly InputAction m_Gameplay_MoveLeft;
        private readonly InputAction m_Gameplay_MoveRight;
        private readonly InputAction m_Gameplay_SoftDrop;
        private readonly InputAction m_Gameplay_HardDrop;
        private readonly InputAction m_Gameplay_RotateCW;
        private readonly InputAction m_Gameplay_RotateCCW;
        private readonly InputAction m_Gameplay_Pause;

        public GameplayActions @Gameplay => new GameplayActions(this);

        public struct GameplayActions
        {
            private @TetrisInputActions m_Wrapper;

            public GameplayActions(@TetrisInputActions wrapper) { m_Wrapper = wrapper; }

            public InputAction @MoveLeft  => m_Wrapper.m_Gameplay_MoveLeft;
            public InputAction @MoveRight => m_Wrapper.m_Gameplay_MoveRight;
            public InputAction @SoftDrop  => m_Wrapper.m_Gameplay_SoftDrop;
            public InputAction @HardDrop  => m_Wrapper.m_Gameplay_HardDrop;
            public InputAction @RotateCW  => m_Wrapper.m_Gameplay_RotateCW;
            public InputAction @RotateCCW => m_Wrapper.m_Gameplay_RotateCCW;
            public InputAction @Pause     => m_Wrapper.m_Gameplay_Pause;

            public InputActionMap Get() => m_Wrapper.m_Gameplay;
            public void Enable()        => Get().Enable();
            public void Disable()       => Get().Disable();
            public bool enabled         => Get().enabled;

            public static implicit operator InputActionMap(GameplayActions set) => set.Get();
        }
    }
}
