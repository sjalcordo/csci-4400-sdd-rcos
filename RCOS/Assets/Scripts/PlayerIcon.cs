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

    /// <summary>
    /// Sets the lobby handler.
    /// </summary>
    /// <param name="handler"></param>
    public void SetLobbyHandler(LobbyHandler handler)
    {
        _lobbyHandler = handler;
    }

    /// <summary>
    /// Sets the hashed ip for the icon.
    /// </summary>
    /// <param name="hash"></param>
    public void SetHashedIP(string hash)
    {
        _hashedIP = hash;
    }

    /// <summary>
    /// Sets the name for the player.
    /// </summary>
    /// <param name="name"></param>
    public void SetName(string name)
    {
        _nameObj.text = name;
    }

    /// <summary>
    /// Sets the profile picture from a given texture.
    /// </summary>
    /// <param name="pfp"></param>
    public void SetPfp(Texture pfp)
    {
        _pfp.texture = pfp;
    }

    /// <summary>
    /// Removes a player from the lobby.
    /// </summary>
    public void RemovePlayer()
    {
        _lobbyHandler.RemovePlayer(_hashedIP);
        Destroy(gameObject);
    }
}
