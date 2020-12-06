using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSound;
    Boolean collisionsAreEnabled = true;

    [SerializeField] float rcsThrust = 30f;
    [SerializeField] float mainThrust = 65f;
    [SerializeField] float levelLoadDelay = 1f;

    [SerializeField] AudioClip mainEngineSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip nextLevelSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem nextLevelParticles;
    enum State { Alive, Dying, Transcending };
    State state = State.Alive;

    int currentLevel;
    int maxLevel;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSound = GetComponent<AudioSource>();
        currentLevel = SceneManager.GetActiveScene().buildIndex;
        maxLevel = SceneManager.sceneCount;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
            if (Debug.isDebugBuild)
            {
                RespondToDebugKeys();
            }
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || !collisionsAreEnabled) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // do nothing
                break;

            case "Finish":
                state = State.Transcending;
                audioSound.Stop();
                audioSound.PlayOneShot(nextLevelSound);
                nextLevelParticles.Play();
                if (currentLevel != maxLevel)
                {
                    Invoke("LoadNextScene", levelLoadDelay);
                }
                break;


            default:
                state = State.Dying;
                audioSound.Stop();
                audioSound.PlayOneShot(deathSound);
                deathParticles.Play();
                Invoke("LoadFirstLevel", levelLoadDelay);
                break;
        }
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        int nextLevel = currentLevel + 1;
        SceneManager.LoadScene(nextLevel);
    }

    private void RespondToRotateInput()
    {
        rigidBody.angularVelocity = Vector3.zero; // Remove rotation due to physics

        float rotationSpeed = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back * rotationSpeed);
        }
    }

    private void RespondToThrustInput()
    {

        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThust();
        }
        else
        {
            audioSound.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThust()
    {
        float thrustSpeed = mainThrust * Time.deltaTime;
        rigidBody.AddRelativeForce(Vector3.up * thrustSpeed);
        mainEngineParticles.Play();
        if (!audioSound.isPlaying)
        {
            audioSound.PlayOneShot(mainEngineSound);
        }
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsAreEnabled = false;
        }
    }
}
