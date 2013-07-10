using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OldSkull.GameLevel;
using Monocle;
using Microsoft.Xna.Framework;

namespace OldSkull.Isle
{
    public class Transition
    {
        public Transition()
        {

        }

        public static Entity TransitionOut(Scene level, int layer)
        {
            Entity trans = new Entity(layer);
            Image transImage = new Image(OldSkullGame.Atlas["ui/transition"]);
            trans.Add(transImage);
            level.Add(trans);
            trans.X = -40;
            Tween.Position(trans, new Vector2(-transImage.Width - 10, 0), 30, Ease.CubeOut, Tween.TweenMode.Oneshot).OnComplete = (Tween tween) =>
            {
                transImage.RemoveSelf();
            };

            return trans;
        }

        public static Entity TransitionIn(Scene level, int layer, Action OnComplete)
        {
            Entity trans = new Entity(layer);
            Image transImage = new Image(OldSkullGame.Atlas["ui/transition"]);
            trans.Add(transImage);
            level.Add(trans);
            trans.X = transImage.Width;
            Tween.Position(trans, new Vector2(-40, 0), 25, Ease.CubeOut, Tween.TweenMode.Oneshot).OnComplete = (Tween tween) =>
            {
                transImage.RemoveSelf();
                OnComplete();
            };

            return trans;
        }
    }
}
