using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Canute.Assets.Script.Module
{
    class GameBackgroundMusic : MonoBehaviour
    {
        public static GameBackgroundMusic instance;
        public AudioSource audioSource;

        public AudioClip start;
        public AudioClip normal;
        public AudioClip battle;

        public bool normalStatus = true;

        public void Awake()
        {
            if (!instance)
                instance = this;
            else
            {
                Destroy(audioSource);
                Destroy(this);
            }
        }

        public void Start()
        {
            DontDestroyOnLoad(this);
        }

        public void Update()
        {
            audioSource.volume = Game.Configuration.Volume;


            if (normalStatus && SceneManager.GetActiveScene().name == "Game Start")
            {
                normalStatus = false;
                audioSource.enabled = false;
                return;
            }

            if (normalStatus && SceneManager.GetActiveScene().name == "Battle")
            {
                normalStatus = false;
                if (audioSource.clip != battle)
                {
                    audioSource.enabled = false;
                    audioSource.enabled = true;
                    audioSource.clip = battle;
                    audioSource.Play();
                }
                return;
            }

            if (!normalStatus && (SceneManager.GetActiveScene().name != "Battle" && SceneManager.GetActiveScene().name != "Game Start"))
            {
                normalStatus = true;
                audioSource.enabled = true;
                if (audioSource.clip != normal)
                {
                    audioSource.enabled = false;
                    audioSource.enabled = true;
                    audioSource.clip = normal;
                    audioSource.PlayDelayed(3);
                }
            }
        }
    }
}
