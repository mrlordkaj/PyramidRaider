using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline;
using SkinnedModel;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace SkinnedModelPipeline
{
    /// <summary>
    /// Writes out a KeyframeContent object to an XNB file to be read in as
    /// a Keyframe.
    /// </summary>
    [ContentTypeWriter]
    class AnimationClipWriter : ContentTypeWriter<AnimationClip>
    {
        protected override void Write(ContentWriter output, AnimationClip value)
        {
            // write duration
            WriteDuration(output, value.Duration);
            WriteKeyframes(output, value.Keyframes);
        }

        private void WriteDuration(ContentWriter output, TimeSpan duration)
        {
            output.Write(duration.Ticks);
        }

        private void WriteKeyframes(ContentWriter output, IList<Keyframe> keyframes)
        {
            Int32 count = keyframes.Count;
            output.Write((Int32)count);

            for (int i = 0; i < count; i++)
            {
                Keyframe keyframe = keyframes[i];
                output.Write(keyframe.Bone);
                output.Write(keyframe.Time.Ticks);
                output.Write(keyframe.Transform);
            }

            return;
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "SkinnedModel.AnimationClip, SkinnedModel";
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "SkinnedModel.AnimationClipReader, SkinnedModel";
        }
    }
}
