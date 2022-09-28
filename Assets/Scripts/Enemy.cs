﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    private Player _player;
    private Animator _enemyDestroyAnimator;
    [SerializeField]
    private AudioClip _explosionSound;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        if (_player == null)
        {
            Debug.LogError("Player is NULL.");
        }

        _enemyDestroyAnimator = this.gameObject.GetComponent<Animator>();

        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            Debug.LogError("Enemy audio source is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if (transform.position.y < -5f)
        {
            float randomX = Random.Range(-8.0f, 8.0f);
            transform.position = new Vector3(randomX, 7, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }
            _enemyDestroyAnimator.SetTrigger("OnEnemyDeath");
            _speed = 0;
            Destroy(this.gameObject, _enemyDestroyAnimator.GetCurrentAnimatorStateInfo(0).length);
        }

        if (other.tag == "Laser")
        {
            _enemyDestroyAnimator.SetTrigger("OnEnemyDeath");
            _speed = 0;
            Destroy(other.gameObject);
            Destroy(this.gameObject, _enemyDestroyAnimator.GetCurrentAnimatorStateInfo(0).length);
            if (_player != null)
            {
                _player.AddScore(10);
            }
        }
        _audioSource.Play();
    }
}
