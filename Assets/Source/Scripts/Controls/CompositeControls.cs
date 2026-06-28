using BananaParty.VehicleGame;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CompositeControls : IControls
{
    private readonly List<IControls> _controls;

    public CompositeControls(IEnumerable<IControls> controls)
    {
        _controls = controls.ToList();
    }

    public float Throttle => Mathf.Clamp(_controls.Sum(control => control.Throttle), -1f, 1f);
    public float Roll => Mathf.Clamp(_controls.Sum(control => control.Roll), -1f, 1f);
    public float Pitch => Mathf.Clamp(_controls.Sum(control => control.Pitch), -1f, 1f);
    public float Yaw => Mathf.Clamp(_controls.Sum(control => control.Yaw), -1f, 1f);
    public bool FirePrimary => _controls.Any(control => control.FirePrimary);
    public bool FireSecondary => _controls.Any(control => control.FireSecondary);
    public bool BackViewCamera => _controls.Any(control => control.BackViewCamera);
    public bool SwitchCamera => _controls.Any(control => control.SwitchCamera);

    public void ManualUpdate()
    {
        foreach (IControls control in _controls)
            control.ManualUpdate();
    }
}
