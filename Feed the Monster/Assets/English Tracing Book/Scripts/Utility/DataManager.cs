using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IndieStudio.EnglishTracingBook.Game;

/*
 * English Tracing Book Package
 *
 * @license		    Unity Asset Store EULA https://unity3d.com/legal/as_terms
 * @author		    Indie Studio - Baraa Nasser
 * @Website		    https://indiestd.com
 * @Asset Store     https://www.assetstore.unity3d.com/en/#!/publisher/9268
 * @Unity Connect   https://connect.unity.com/u/5822191d090915001dbaf653/column
 * @email		    info@indiestd.com
 *
 */

namespace IndieStudio.EnglishTracingBook.Utility
{
    public class DataManager
    {
        private static readonly string prefix = "EnglishTracingBook-";


        /// <summary>
        /// Save the number of stars of the given shape
        /// </summary>
        /// <param name="ID">The ID of the shape.</param>
        /// <param name="stars">The stars of the shape.</param>
        /// <param name="shapesManager">Shapes manager reference.</param>
        public static void SaveShapeStars(int ID, ShapesManager.Shape.StarsNumber stars, ShapesManager shapesManager)
        {
            PlayerPrefs.SetInt(GetStarsStrKey(ID, shapesManager), CommonUtil.ShapeStarsNumberEnumToIntNumber(stars));
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Save the color of the path.
        /// </summary>
        /// <param name="Shape ID">The ID of the shape.</param>
        /// <param name="compundID">Compund ID.</param>
        /// <param name="from">From number.</param>
        /// <param name="to">To number.</param>
        /// <param name="color">Color value.</param>
        /// <param name="shapesManager">Shapes manager.</param>
        public static void SaveShapePathColor(int shapeID, int compundID, int from, int to, Color color, ShapesManager shapesManager)
        {
            string key = GetPathStrKey(shapeID, compundID, from, to, shapesManager);
            string value = color.r + "," + color.g + "," + color.b + "," + color.a;

            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Save the shape locked status.
        /// </summary>
        /// <param name="ID">The ID of the shape.</param>
        /// <param name="isLocked">Whether the shape is locked or not.</param>
        /// <param name="shapesManager">Shapes manager reference.</param>
        public static void SaveShapeLockedStatus(int ID, bool isLocked, ShapesManager shapesManager)
        {
            PlayerPrefs.SetInt(GetLockedStrKey(ID, shapesManager), CommonUtil.TrueFalseBoolToZeroOne(isLocked));
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Save the music mute value
        /// </summary>
        /// <param name="count">The music mute value.</param>
        public static void SaveMusicMuteValue(int value)
        {
            PlayerPrefs.SetInt(GetMusicMuteStrKey(), value);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Save the sfx mute value
        /// </summary>
        /// <param name="count">The sfx mute value.</param>
        public static void SaveSFXMuteValue(int value)
        {
            PlayerPrefs.SetInt(GetSFXMuteStrKey(), value);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Save the help count value
        /// </summary>
        /// <param name="count">The help count value.</param>
        public static void SaveHelpCountValue(int value)
        {
            PlayerPrefs.SetInt(GetHelpCountStrKey(), value);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Save the tracing mode value
        /// </summary>
        /// <param name="count">The tracing mode value.</param>
        public static void SaveTracingModeValue(int value)
        {
            PlayerPrefs.SetInt(GetTracingModeStrKey(), value);
            PlayerPrefs.Save();
        }


        /// <summary>
        /// Get the number of stars of the given shape
        /// </summary>
        /// <returns>The shape stars.</returns>
        /// <param name="ID">The ID of the shape.</param>
        /// <param name="shapesManager">Shapes manager reference.</param>
        public static ShapesManager.Shape.StarsNumber GetShapeStars(int ID, ShapesManager shapesManager)
        {
            ShapesManager.Shape.StarsNumber stars = ShapesManager.Shape.StarsNumber.ZERO;
            string key = GetStarsStrKey(ID, shapesManager);
            if (PlayerPrefs.HasKey(key))
            {
                stars = CommonUtil.IntNumberToShapeStarsNumberEnum(PlayerPrefs.GetInt(key));
            }
            return stars;
        }

        /// <summary>
        /// Get the color of the shape path.
        /// </summary>
        /// <returns>The shape path color.</returns>
        /// <param name="Shape ID">The ID of the shape.</param>
        /// <param name="compundID">Compund ID.</param>
        /// <param name="from">From number.</param>
        /// <param name="to">To number.</param>
        /// <param name="shapesManager">Shapes manager reference.</param>
        public static Color GetShapePathColor(int shapeID, int compundID, int from, int to, ShapesManager shapesManager)
        {
            Color color = Color.white;
            string key = GetPathStrKey(shapeID, compundID, from, to, shapesManager);

            if (PlayerPrefs.HasKey(key))
            {
                color = CommonUtil.StringRGBAToColor(PlayerPrefs.GetString(key));
            }

            return color;
        }


        /// <summary>
        /// Determine if the shape is locked or not.
        /// </summary>
        /// <returns><c>true</c> if shape is locked; otherwise, <c>false</c>.</returns>
        /// <param name="ID">The ID of the shape.</param>
        /// <param name="shapesManager">Shapes manager reference.</param>
        public static bool IsShapeLocked(int ID, ShapesManager shapesManager)
        {
            bool isLocked = true;
            string key = GetLockedStrKey(ID, shapesManager);
            if (PlayerPrefs.HasKey(key))
            {
                isLocked = CommonUtil.ZeroOneToTrueFalseBool(PlayerPrefs.GetInt(key));
            }
            return isLocked;
        }


        /// <summary>
        /// Get the collected stars of the shapes in the given shapes manager.
        /// </summary>
        /// <returns>The collected stars.</returns>
        /// <param name="shapesManager">Shapes manager reference.</param>
        public static int GetCollectedStars(ShapesManager shapesManager)
        {

            int ID = 0;
            int cs = 0;
            for (int i = 0; i < shapesManager.shapes.Count; i++)
            {
                ID = (i + 1);
                ShapesManager.Shape.StarsNumber sn = GetShapeStars(ID, shapesManager);
                if (sn == ShapesManager.Shape.StarsNumber.ONE)
                {
                    cs += 1;
                }
                else if (sn == ShapesManager.Shape.StarsNumber.TWO)
                {
                    cs += 2;
                }
                else if (sn == ShapesManager.Shape.StarsNumber.THREE)
                {
                    cs += 3;
                }
            }
            return cs;
        }

        /// <summary>
        /// Load the music mute value.
        /// </summary>
        /// <returns>The music mute value.</returns>
        public static bool GetMusicMuteValue()
        {
            bool value = false;
            if (PlayerPrefs.HasKey(GetMusicMuteStrKey()))
            {
                value = CommonUtil.ZeroOneToTrueFalseBool(PlayerPrefs.GetInt(GetMusicMuteStrKey()));
            }
            return value;
        }

        /// <summary>
        /// Load the sfx mute value.
        /// </summary>
        /// <returns>The sfx mute value.</returns>
        public static bool GetSFXMuteValue()
        {
            bool value = false;
            if (PlayerPrefs.HasKey(GetSFXMuteStrKey()))
            {
                value = CommonUtil.ZeroOneToTrueFalseBool(PlayerPrefs.GetInt(GetSFXMuteStrKey()));
            }
            return value;
        }

        /// <summary>
        /// Load the help count value.
        /// </summary>
        /// <returns>The help count value.</returns>
        public static int GetHelpCountValue()
        {
            int value = -1;
            if (PlayerPrefs.HasKey(GetHelpCountStrKey()))
            {
                value = PlayerPrefs.GetInt(GetHelpCountStrKey());
            }
            return value;
        }

        /// <summary>
        /// Load the tracing mode value.
        /// </summary>
        /// <returns>The tracing mode value.</returns>
        public static int GetTracingModeValue()
        {
            int value = 1;//default set Line
            if (PlayerPrefs.HasKey(GetTracingModeStrKey()))
            {
                value = PlayerPrefs.GetInt(GetTracingModeStrKey());
            }

            return value;
        }

        /// <summary>
        /// Return the string key of specific path.
        /// </summary>
        /// <returns>The string key.</returns>
        /// <param name="shapeID">The ID of the shape.</param>
        /// <param name="compundID">Compund ID.</param>
        /// <param name="from">From number.</param>
        /// <param name="to">To number.</param>
        /// <param name="shapesManager">Shapes manager reference.</param>
        public static string GetPathStrKey(int shapeID, int compundID, int from, int to, ShapesManager shapesManager)
        {
            return prefix + shapesManager.shapePrefix + "-Shape-" + shapeID + "-Compound-" + compundID + "-Path-" + from + "-" + to;
        }

        /// <summary>
        /// Return the locked string key of specific shape.
        /// </summary>
        /// <returns>The locked string key.</returns>
        /// <param name="ID">The ID of the shape.</param>
        /// <param name="shapesManager">Shapes manager reference.</param>
        public static string GetLockedStrKey(int ID, ShapesManager shapesManager)
        {
            return prefix + shapesManager.shapePrefix + "-Shape-" + ID + "-isLocked";
        }

        /// <summary>
        /// Return the stars string key of specific shape.
        /// </summary>
        /// <returns>The stars string key.</returns>
        /// <param name="ID">The ID of the shape.</param>
        /// <param name="shapesManager">Shapes manager reference.</param>
        public static string GetStarsStrKey(int ID, ShapesManager shapesManager)
        {
            return prefix + shapesManager.shapePrefix + "-Shape-" + ID + "-Stars";
        }

        /// <summary>
        /// Return the music mute string key.
        /// </summary>
        /// <returns>The music mute key.</returns>
        private static string GetMusicMuteStrKey()
        {
            return prefix + "-MusicMute";
        }

        /// <summary>
        /// Return the sfx mute string key.
        /// </summary>
        /// <returns>The sfx mute key.</returns>
        private static string GetSFXMuteStrKey()
        {
            return prefix + "-SFXMute";
        }

        /// <summary>
        /// Return the help count string key.
        /// </summary>
        /// <returns>The help count key.</returns>
        private static string GetHelpCountStrKey()
        {
            return prefix + "-HelpCount";
        }

        /// <summary>
        /// Return the tracing mode string key
        /// </summary>
        /// <returns>The tracing mode key</returns>
        private static string GetTracingModeStrKey()
        {
            return prefix + "-TracingMode";
        }

        /// <summary>
        /// Reset the game.
        /// </summary>
        public static void ResetGame()
        {
            //Load temp values
            PlayerPrefs.DeleteAll();
            //Save temp values again
        }
    }
}