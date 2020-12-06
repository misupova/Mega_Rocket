using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSound;
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

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; }

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
                Invoke("LoadNextScene", levelLoadDelay);
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
        SceneManager.LoadScene(1);
    }

    private void RespondToRotateInput()
    {
        float rotationSpeed = rcsThrust * Time.deltaTime;
        rigidBody.freezeRotation = true;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back * rotationSpeed);
        }
        rigidBody.freezeRotation = false;
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
}
