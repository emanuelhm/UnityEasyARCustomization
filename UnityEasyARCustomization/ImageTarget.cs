using easyar;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEasyARCustomization
{
    public class ImageTarget : ImageTargetController
    {
        public UnityEvent OnActive = null;

        public Sprite Sprite = null;

        private byte[] _imageData;

        public void Initialize()
        {
            _imageData = Sprite.texture.EncodeToPNG();

            switch (targetType)
            {
                case TargetType.LocalImage:
                case TargetType.LocalTargetData:
                    if (gameObject.activeSelf)
                        StartCoroutine(LoadImageTarget());
                    break;
                case TargetType.Cloud:
                    if (target != null)
                    {
                        TargetName = target.name();
                    }
                    break;
            }
        }

        public override void Start() { }

        private IEnumerator LoadImageTarget()
        {
            var path = TargetPath;
            var type = Type;

            var data = _imageData;
            Buffer buffer = Buffer.create(data.Length);
            var ptr = buffer.data();
            System.Runtime.InteropServices.Marshal.Copy(data, 0, ptr, data.Length);

            Optional<easyar.ImageTarget> op_target;
            if (targetType == TargetType.LocalImage)
            {
                var image = ImageHelper.decode(buffer);
                if (!image.OnSome)
                {
                    throw new System.Exception("decode image file data failed");
                }

                var p = new ImageTargetParameters();
                p.setImage(image.Value);
                p.setName(TargetName);
                p.setScale(TargetSize);
                p.setUid("");
                p.setMeta("");
                op_target = easyar.ImageTarget.createFromParameters(p);

                if (!op_target.OnSome)
                {
                    throw new System.Exception("create image target failed from image target parameters");
                }

                image.Value.Dispose();
                buffer.Dispose();
                p.Dispose();
            }
            else
            {
                op_target = easyar.ImageTarget.createFromTargetData(buffer);

                if (!op_target.OnSome)
                {
                    throw new System.Exception("create image target failed from image target target data");
                }

                buffer.Dispose();
            }

            target = op_target.Value;
            if (ImageTracker == null)
            {
                yield break;
            }
            ImageTracker.LoadImageTarget(this, (_target, status) =>
            {
                targetImage = ((_target as easyar.ImageTarget).images())[0];
                Debug.Log("[EasyAR] Targtet name: " + _target.name() + " target runtimeID: " + _target.runtimeID() + " load status: " + status);
                Debug.Log("[EasyAR] Target size" + targetImage.width() + " " + targetImage.height());
                OnActive.Invoke();
                gameObject.SetActive(false);
            });
        }

        public override void OnTracking(Matrix4x4 pose)
        {
            transform.localScale = transform.localScale * TargetSize;
        }
    }
}