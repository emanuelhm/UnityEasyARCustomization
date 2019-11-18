using easyar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityEasyARCustomization
{
    class ImageTrackerBehaviour : easyar.ImageTrackerBehaviour
    {
        public override void UpdateFrame(ARSessionUpdateEventArgs args)
        {
            List<ImageTargetController> currentTrackingControllers = new List<ImageTargetController>();
            var results = args.OFrame.results();

            foreach (var _result in results)
            {
                ImageTrackerResult result = null;
                if (_result.OnSome)
                {
                    result = _result.Value as ImageTrackerResult;
                }

                if (result != null)
                {
                    var targetInstances = result.targetInstances();
                    int centerTargetId = -1;
                    if (TargetCamera == null)
                    {
                        Utility.SetMatrixOnTransform(Camera.main.transform, centerTransform);
                    }
                    else
                    {
                        Utility.SetMatrixOnTransform(TargetCamera.transform, centerTransform);
                    }
                    if (CenterImageTarget != null && CenterImageTarget.Target() != null && CenterTarget == CenterMode.SpecificTarget)
                    {
                        centerTargetId = CenterImageTarget.Target().runtimeID();
                    }
                    foreach (var targetInstance in targetInstances)
                    {
                        var target = targetInstance.target();
                        if (!target.OnSome)
                        {
                            continue;
                        }
                        var status = targetInstance.status();
                        foreach (var targetController in targetControllers)
                        {
                            var _target = targetController.Target();
                            if (target.Value.runtimeID() == _target.runtimeID())
                            {
                                if (status == TargetStatus.Tracked)
                                {
                                    if (!targetController.Tracked)
                                    {
                                        targetController.OnFound();
                                        targetController.Tracked = true;
                                    }
                                    var pose = Utility.Matrix44FToMatrix4x4(targetInstance.pose());

                                    pose = args.ImageRotationMatrixGlobal * pose * Matrix4x4.Rotate(Quaternion.Euler(-90, 0, 180));

                                    if (CenterTarget == CenterMode.FirstTarget && centerTargetId == -1)
                                    {
                                        centerTargetId = target.Value.runtimeID();
                                        CenterImageTarget = targetController;
                                    }

                                    if (centerTargetId != target.Value.runtimeID())
                                    {
                                        pose = centerTransform * pose;
                                        targetController.OnTracking(pose);
                                    }
                                    else
                                    {
                                        targetController.OnTracking(Matrix4x4.identity);
                                        centerTransform = pose.inverse;
                                    }
                                    currentTrackingControllers.Add(targetController);
                                }
                            }
                        }
                        target.Value.Dispose();
                        targetInstance.Dispose();
                    }
                    result.Dispose();
                }
            }
            foreach (var targetController in targetControllers)
            {
                bool contain = false;
                foreach (var item in currentTrackingControllers)
                {
                    if (item == targetController)
                    {
                        contain = true;
                    }
                }
                if (!contain && targetController.Tracked)
                {
                    targetController.OnLost();
                    targetController.Tracked = false;
                }
            }
        }
    }
}