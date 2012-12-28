﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calcifer.Engine.Scenegraph;
using ComponentKit.Model;
using OpenTK;

namespace Calcifer.Engine.Components
{
    public class CameraComponent: DependencyComponent, ICamera
    {
        [RequireComponent] private TransformComponent transform;

        public static CameraComponent Current { get; private set; }
        public Matrix4 Matrix 
        { 
            get
            {
                var eye = transform.Translation;
                var target = eye + Vector3.Transform(Vector3.UnitX, transform.Rotation);
                var up = Vector3.Transform(Vector3.UnitZ, transform.Rotation);
                return Matrix4.LookAt(eye, target, Vector3.UnitZ);
            } 
        }

        public CameraComponent()
        {
            if (Current == null) MakeActive();
        }

        public void MakeActive()
        {
            Current = this;
        }
    }
}
