using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace SkinnedModel
{
    /// <summary>
    /// A custom reader to read Keyframe.
    /// </summary>
    public class AnimationClipReader : ContentTypeReader<AnimationClip>
    {
        protected override AnimationClip Read(ContentReader input, AnimationClip existingInstance)
        {
            AnimationClip animationClip = existingInstance;

            if (existingInstance == null)
            {
                TimeSpan duration = ReadDuration(input);
                List<Keyframe> keyframes = ReadKeyframes(input, null);
                animationClip = new AnimationClip(duration, keyframes);
            }
            else
            {
                animationClip.Duration = ReadDuration(input);
                ReadKeyframes(input, animationClip.Keyframes);
            }
            return animationClip;
        }

        private TimeSpan ReadDuration(ContentReader input)
        {
            return new TimeSpan(input.ReadInt64());
        }

        private List<Keyframe> ReadKeyframes(ContentReader input, List<Keyframe> existingInstance)
        {
            List<Keyframe> keyframes = existingInstance;

            int count = input.ReadInt32();
            if (keyframes == null)
                keyframes = new List<Keyframe>(count);

            for (int i = 0; i < count; i++)
            {
                Keyframe keyframe = new Keyframe();
                keyframe.Bone = input.ReadInt32();
                keyframe.Time = new TimeSpan(input.ReadInt64());
                keyframe.Transform = input.ReadMatrix();
                if (existingInstance == null)
                    keyframes.Add(keyframe);
                else
                    keyframes[i] = keyframe;
            }
            return keyframes;
        }
    }
}
