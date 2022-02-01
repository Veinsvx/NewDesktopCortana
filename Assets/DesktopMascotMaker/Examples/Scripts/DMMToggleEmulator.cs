using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Windows.Forms;
using System.Drawing;
using DesktopMascotMaker;

public class DMMToggleEmulator : MonoBehaviour
{
    public MascotMakerMulti mascotMakerMulti;
    public UnityEngine.UI.Toggle toggle;

    // toggle offset position relative to MascotMaker.Instance.location
    public Vector2 Offset = new Vector2(100, 100);

    [SerializeField]
    private UnityEngine.Events.UnityEvent leftMouseDownEvent = new UnityEngine.Events.UnityEvent();

    void Start()
    {
        // mascotMakerMulti must not be null
        Debug.Assert(mascotMakerMulti != null, "mascotMakerMulti != null", transform);
        Debug.Assert(toggle != null, "toggle != null", transform);
        
        // Assign custom function to MascotMakerMulti's EventHandler
        mascotMakerMulti.OnLeftMouseDown += LeftMouseDown;
    }

    // Click Event
    public void LeftMouseDown(object sender, MouseEventArgs e)
    {
        toggle.isOn = !toggle.isOn;

        leftMouseDownEvent.Invoke();
    }

    void Update()
    {
        mascotMakerMulti.Location = Point.Add(MascotMaker.Instance.Location, new Size((int)Offset.x, (int)Offset.y));
    }
}
