using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

//Classe che rappresenta un certificato contenente la chiave di sessione di cifratura
//sono presenti sia metodi Get sia metodi Set in quanto questo certificato viene costruito man mano che si ricevono i messaggi
public class CacheKey
{
    private string masterNonce;
    private string spiderNonce;
    private byte[] sessionKey;
    private byte[] sessionIV;
    private bool isAuth;

    public void SetMasterNonce(string masterNonce)
    {
        this.masterNonce = masterNonce;
    }

    public string GetMasterNonce()
    {
        return masterNonce;
    }

    public void SetSpiderNonce(string spiderNonce)
    {
        this.spiderNonce = spiderNonce;
    }

    public string GetSpiderNonce()
    {
        return spiderNonce;
    }

    public void SetSessionKey(byte[] sessionKey)
    {
        this.sessionKey = sessionKey;
    }

    public byte[] GetSessionKey()
    {
        return sessionKey;
    }

    public void SetSessionIV(byte[] sessionIV)
    {
        this.sessionIV = sessionIV;
    }

    public byte[] GetSessionIV()
    {
        return sessionIV;
    }

    public void SetIsAuth(bool isAuth)
    {
        this.isAuth = isAuth;
    }

    public bool IsAuth()
    {
        return isAuth;
    }
}
