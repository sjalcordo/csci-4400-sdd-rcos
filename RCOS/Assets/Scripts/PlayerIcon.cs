/*
 *  AUTHOR: Sean (alcors@rpi.edu)
 *  DESC: Helper class used to easily set the text and picture of a player icon.
 */

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Gameplay;

public class PlayerIcon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text _nameObj;
    [SerializeField] private RawImage _pfp;

    // Member Variables
    private string _hashedIP;
    private LobbyHandler _lobbyHandler;

    public void SetLobbyHandler(LobbyHandler handler)
    {
        _lobbyHandler = handler;
    }

    public void SetHashedIP(string hash)
    {
        _hashedIP = hash;
    }

    public void SetName(string name)
    {
        _nameObj.text = name;
    }

    public void SetPfp(Texture pfp)
    {
        _pfp.texture = pfp;
    }

    public void RemovePlayer()
    {
        _lobbyHandler.RemovePlayer(_hashedIP);
        Destroy(gameObject);
    }
}
