using System.Collections;
using System.Collections.Generic;
using Lib.Physics;
using Lib.Sound;
using Lib.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMainController : MonoBehaviour
{
    [SerializeField] private PlayerBody     _player;
    [SerializeField] private PlayerCamera   _playerCam;
    [SerializeField] private GameObject     _checkPointRoot;
    [SerializeField] private CheckPointText _checkPointText;
    [SerializeField] private CheckPointText _readyText;
    [SerializeField] private CheckPointText _goalText;
    [SerializeField] private GameObject     _titleButton;

    private StateMachine    _state;
    private SimpleTimer     _timer = new SimpleTimer();
    private Spring2DManager _springManager;
    private float           _checkPointX = 0.0f;
    

    void Awake()
    {
        _springManager = new Spring2DManager();
        _state = new StateMachine(State_Ready);

        _player.Create(this, _springManager, _player.transform.position);

        var check = _checkPointRoot.GetComponentsInChildren<CheckPoint>();
        foreach(var i in check)
        {
            i.Create(_springManager);
        }
    }

    void Update()
    {
        _state.Update();
    }

    void FixedUpdate()
    {
        if(_state.Curr == State_Damage)
        {
            return;
        }
        // バネ処理
        _springManager.Stretch(Time.fixedDeltaTime);
    }

    /// <summary>
    /// ゲーム開始時の待ち
    /// </summary>
    private void State_Ready(StateMachineCase c)
    {
        switch(c)
        {
            case StateMachineCase.Enter:
            {
                _timer.Init(1.0f);
            }
            break;
            case StateMachineCase.Exec:
            {
                 _player.InputUpdate(false);
                if(_timer.Update())
                {
                    _state.ChangeState(State_Play);
                }
            }
            break;
            case StateMachineCase.Exit:
            {
                _readyText.Show(Vector2.zero);
                SoundManager.Instance.PlayBGM(BGMPath._SUNADOKEISEIUN);
                SoundManager.Instance.PlaySE(SEPath._READY_GO);
            }
            break;
        }
    }

    /// <summary>
    /// 通常プレイ中
    /// </summary>
    private void State_Play(StateMachineCase c)
    {
        switch(c)
        {
            case StateMachineCase.Enter:
            break;
            case StateMachineCase.Exec:
            {
                _player.InputUpdate(true);
            }
            break;
            case StateMachineCase.Exit:
            break;
        }
    }

    /// <summary>
    /// プレイヤーのダメージ & リスポーン
    /// </summary>
    private void State_Damage(StateMachineCase c)
    {
        switch(c)
        {
            case StateMachineCase.Enter:
            {
                _timer.Init(1.0f);
            }
            break;
            case StateMachineCase.Exec:
            {
                if(_timer.Update())
                {
                    _state.ChangeState(State_Respawn);
                }
            }
            break;
            case StateMachineCase.Exit:
            {
            }
            break;
        }
    }

    private void State_Respawn(StateMachineCase c)
    {
        switch(c)
        {
            case StateMachineCase.Enter:
            {
                _player.Reset(new Vector2(_checkPointX, 0.0f));
                _timer.Init(0.1f);
            }
            break;
            case StateMachineCase.Exec:
            {
                 _player.InputUpdate(false);
                if(_timer.Update())
                {
                    _state.ChangeState(State_Play);
                }
            }
            break;
            case StateMachineCase.Exit:
            {
            }
            break;
        }
    }

    /// <summary>
    /// ゴール
    /// </summary>
    private void State_Goal(StateMachineCase c)
    {
        switch(c)
        {
            case StateMachineCase.Enter:
            {
                _goalText.Show(_goalText.transform.position);
                _playerCam.SetTarget(_goalText.transform);

                _timer.Init(6.0f);
            }
            break;
            case StateMachineCase.Exec:
            {
                _timer.Update();
                _titleButton.SetActive(_timer.IsEnd());
                _player.InputUpdate(true);
            }
            break;
            case StateMachineCase.Exit:
            {
            }
            break;
        }
    }

    /// <summary>
    /// プレイヤーのダメージ通知
    /// </summary>
    public void OnPlayerDamage()
    {
        if(_state.Next == State_Damage) { return; }
        if(_state.Curr != State_Play) { return; }
        
        _state.ChangeState(State_Damage);
        _playerCam.Shake(_player.body.velocity);
        SoundManager.Instance.PlaySE(SEPath._DAMAGE);
    }

    /// <summary>
    /// チェックポイント通過
    /// </summary>
    public void CheckPoint(float xx)
    {
        if(Mathf.Abs(xx - _checkPointX) > 1.0f)
        {
            // 前通過したチェックポイントと違えばテキスト表示
            Vector2 pos = new Vector2(xx + 1.0f, _player.body.position.y);
            _checkPointText.Show(pos);
            _playerCam.Shake(_player.body.velocity, 0.1f);
            SoundManager.Instance.PlaySE(SEPath._CHECK_POINT);
        }
        _checkPointX = xx;

    }

    public void Goal()
    {
        if(_state.Next == State_Goal) { return; }
        if(_state.Curr != State_Play) { return; }
        _state.ChangeState(State_Goal);
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlayJingle(JinglePath._GOAL);
    }

    public bool IsDamage()
    {
        return _state.Curr == State_Damage || _state.Next == State_Damage || _state.Curr == State_Respawn;
    }

    public bool IsGoal()
    {
        return _state.Curr == State_Goal || _state.Next == State_Goal;
    }

    public void OnClick()
    {
        SceneManager.LoadScene("Title");
    }
}
