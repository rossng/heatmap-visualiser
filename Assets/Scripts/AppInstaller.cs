using Zenject;
using UnityEngine;
using System.Collections;

public class AppInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<HeatmapApi>().AsSingle().NonLazy();
    }
}