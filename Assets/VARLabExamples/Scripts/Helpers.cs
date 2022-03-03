/*This source code was originally provided by the Digital Simulations Lab (VARLab) at Conestoga College in Ontario, Canada.
 * It was provided as a foundation of learning for participants of our 2022 Introduction to Unity Boot Camp.
 * Participants are welcome to use, extend and share projects derived from this code under the Creative Commons Attribution-NonCommercial 4.0 International license as linked below:
        Summary: https://creativecommons.org/licenses/by-nc/4.0/
        Full: https://creativecommons.org/licenses/by-nc/4.0/legalcode
 * You may not sell works derived from this code, but we hope you learn from it and share that learning with others.
 * We hope it inspires you to make more games or consider a career in game development.
 * To learn more about the opportunities for computer science and software engineering at Conestoga College please visit https://www.conestogac.on.ca/applied-computer-science-and-information-technology */

using System.Collections.Generic;
using UnityEngine;

namespace TigerTail
{
    public static class Helpers
    {
        public static bool TryGetInterfaces<T>(out List<T> resultList, GameObject objectToSearch) where T : class
        {
            MonoBehaviour[] list = objectToSearch.GetComponents<MonoBehaviour>();
            resultList = new List<T>();
            var found = false;
            foreach (MonoBehaviour mb in list)
            {
                if (mb is T)
                {
                    resultList.Add((T)((System.Object)mb));
                    found = true;
                }
            }

            return found;
        }

        public static bool TryGetInterface<T>(out T result, GameObject objectToSearch) where T : class
        {
            MonoBehaviour[] list = objectToSearch.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour mb in list)
            {
                if (mb is T)
                {
                    result = (T)((System.Object)mb);
                    return true;
                }
            }

            result = null;
            return false;
        }

        public static int Modulo(int k, int n) { return ((k %= n) < 0) ? k + n : k; }
    }
}
