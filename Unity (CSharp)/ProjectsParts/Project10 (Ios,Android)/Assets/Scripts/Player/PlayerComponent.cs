using UnityEngine;

namespace Player.Jack {

  public class PlayerComponent: ExEvent.EventBehaviour {

    private Rigidbody2D _rigidbody;
    public new Rigidbody2D rigidbody {
      get {
        if (_rigidbody == null)
          _rigidbody = GetComponentInChildren<Rigidbody2D>();
        if (_rigidbody == null)
          _rigidbody = GetComponentInParent<Rigidbody2D>();
        return _rigidbody;
      }
    }

    private PlayerController _controller;
    /// <summary>
    /// Основной контроллер персонажа
    /// </summary>
    public PlayerController controller {
      get {
        if (_controller == null)
          _controller = GetComponentInChildren<PlayerController>();
        if (_controller == null)
          _controller = GetComponentInParent<PlayerController>();
        return _controller;
      }
    }

    private PlayerMove _move;
    /// <summary>
    /// Компонент движения
    /// </summary>
    public PlayerMove move {
      get {
        if (_move == null)
          _move = GetComponentInChildren<PlayerMoveManager>().activeController;
        if (_move == null)
          _move = GetComponentInParent<PlayerMoveManager>().activeController;
        return _move;
      }
    }

    private PlayerMoveManager _moveManager;
    /// <summary>
    /// Компонент движения
    /// </summary>
    public PlayerMoveManager moveManager {
      get {
        if (_moveManager == null)
          _moveManager = GetComponentInChildren<PlayerMoveManager>();
        if (_moveManager == null)
          _moveManager = GetComponentInParent<PlayerMoveManager>();
        return _moveManager;
      }
    }

    private PlayerBoosters _boost;
    /// <summary>
    /// Компонент бустов
    /// </summary>
    public PlayerBoosters boost {
      get {
        if (_boost == null)
          _boost = GetComponentInChildren<PlayerBoosters>();
        if (_boost == null)
          _boost = GetComponentInParent<PlayerBoosters>();
        return _boost;
      }
    }

    private PlayerShoot _shoot;
    /// <summary>
    /// Компонент атаки
    /// </summary>
    public PlayerShoot shoot {
      get {
        if (_shoot == null)
          _shoot = GetComponentInChildren<PlayerShoot>();
        if (_shoot == null)
          _shoot = GetComponentInParent<PlayerShoot>();
        return _shoot;
      }
    }

    private PlayerCheck _check;
    /// <summary>
    /// Компонент проверки окружения
    /// </summary>
    public PlayerCheck check {
      get {
        if (_check == null)
          _check = GetComponentInChildren<PlayerCheck>();
        if (_check == null)
          _check = GetComponentInParent<PlayerCheck>();
        return _check;
      }
    }

    private PlayerPets _pet;
    /// <summary>
    /// Компонент петов
    /// </summary>
    public PlayerPets pet {
      get {
        if (_pet == null)
          _pet = GetComponentInChildren<PlayerPets>();
        if (_pet == null)
          _pet = GetComponentInParent<PlayerPets>();
        return _pet;
      }
    }

    private PlayerAudio _audio;
    /// <summary>
    /// Аудио компонент
    /// </summary>
    public new PlayerAudio audio {
      get {
        if (_audio == null)
          _audio = GetComponentInChildren<PlayerAudio>();
        if (_audio == null)
          _audio = GetComponentInParent<PlayerAudio>();
        return _audio;
      }
    }

    private PlayerAnimation _animation;
    /// <summary>
    /// Компонент анимации
    /// </summary>
    public new PlayerAnimation animation {
      get {
        if (_animation == null)
          _animation = GetComponentInChildren<PlayerAnimation>();
        if (_animation == null)
          _animation = GetComponentInParent<PlayerAnimation>();
        return _animation;
      }
    }

  }
}