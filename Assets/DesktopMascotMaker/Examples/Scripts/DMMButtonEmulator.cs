using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Windows.Forms;
using System.Drawing;
using DesktopMascotMaker;

public class DMMButtonEmulator : MonoBehaviour
{
    public MascotMakerMulti mascotMakerMulti;
    public UnityEngine.UI.Button button;
    
    // button offset position relative to MascotMaker.Instance.location
    public Vector2 Offset = new Vector2(100, 100);
    private PointerEventData pointer;

    [SerializeField]
    private UnityEngine.Events.UnityEvent leftMouseDownEvent = new UnityEngine.Events.UnityEvent();

    void Start()
    {
        // mascotMakerMulti must not be null
        Debug.Assert(mascotMakerMulti != null, "mascotMakerMulti != null", transform);
        Debug.Assert(button != null, "button != null", transform);
        
        // Assign custom function to MascotMakerMulti's EventHandler
        mascotMakerMulti.OnLeftMouseDown += LeftMouseDown;

        // pointer event for Execute     
        pointer = new PointerEventData(EventSystem.current); 
    }

    // Click Event
    public void LeftMouseDown(object sender, MouseEventArgs e)
    {
        // Simulate Button presses through code unity 4.6 GUI
        // https://answers.unity.com/questions/820599/simulate-button-presses-through-code-unity-46-gui.html
        ExecuteEvents.Execute(button.gameObject, pointer, ExecuteEvents.submitHandler);
        leftMouseDownEvent.Invoke();
    }

    void Update()
    {
        mascotMakerMulti.Location = Point.Add(MascotMaker.Instance.Location, new Size((int)Offset.x, (int)Offset.y));
    }
}
