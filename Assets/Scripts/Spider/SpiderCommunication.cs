using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class SpiderCommunication : MonoBehaviour
{
    private SpiderManager spiderManager;

    private CacheKey cacheKey;

    private void Start()
    {
        spiderManager = GetComponent<SpiderManager>();

        //Scambio di una nuova chiave di sessione ogni 2 minuti
        InvokeRepeating("SendExchangeMessage", 0, 120);
    }

    private void OnEnable()
    {
        EventManager.OnMessageReceived += HandleMessage;
    }

    private void OnDisable()
    {
        EventManager.OnMessageReceived -= HandleMessage;
    }

    private void HandleMessage(string message)
    {
        //Se il messaggio inizia con "Spider, " è un messaggio per lo Spider Robot, anche se bisognerà controllare l'ID
        if (message.StartsWith("Spider, "))
        {
            message = message.Substring("Spider, ".Length).Trim();
        }
        else
        {
            return;
        }

        //Verifica del protocollo di comunicazione utilizzato dal messaggio
        if (message.StartsWith("Exchange,"))
        {
            message = message.Substring("Exchange,".Length).Trim();
            ReceiveExchangeMessage(message);
            return;
        }
        if (message.StartsWith("Auth,"))
        {
            message = message.Substring("Auth,".Length).Trim();
            ReceiveAuthMessage(message);
            return;
        }
    }

    //Metodo per inviare un messaggio di scambio chiave (EXCH_SP)
    private void SendExchangeMessage()
    {
        //Nuovo certificato della chiave di sessione
        cacheKey = new CacheKey();

        //Certificato della chiave Master
        Certificate certificate = spiderManager.GetCertificate();

        //Se non si è in possesso del certificato della chiave Master, non si può fare lo scambio
        if (certificate == null)
        {
            return;
        }

        //ID dello Spider Robot
        string spiderID = spiderManager.GetSpiderID();

        //Preparazione del messaggio
        string message = $"Master, Exchange, {spiderID}";

        //Nonce generato dallo Spider Robot
        string spiderNonce = CryptographyManager.GenerateRandomUUID();
        cacheKey.SetSpiderNonce(spiderNonce);

        //Cifratura del messaggio con la chiave Master
        string plaintext = $"{spiderID}, {spiderNonce}";
        byte[] plaintextBytes = System.Text.Encoding.UTF8.GetBytes(plaintext);
        byte[] ciphertextBytes = CryptographyManager.AesEncrypt(plaintextBytes, certificate.GetMasterKey(), certificate.GetMasterIV());
        string ciphertext = CryptographyManager.ByteArrayToHexString(ciphertextBytes);

        //Aggiunta del messaggio cifrato al messaggio da inviare
        message += ", " + ciphertext;

        //Invio del messaggio
        EventManager.SendMessage(message);
    }

    private void ReceiveExchangeMessage(string message)
    {
        //Se nel messaggio EXCH_MS non è presente l'ID dello Spider Robot, non è indirizzato a questo Spider Robot
        string spiderID = message.Split(',')[0].Trim();
        if (spiderID != spiderManager.GetSpiderID())
        {
            return;
        }

        Certificate certificate = spiderManager.GetCertificate();
        if (certificate == null)
        {
            return;
        }

        //Decifratura del messaggio con la chiave Master
        string ciphertext = message.Split(',')[1].Trim();
        byte[] ciphertextBytes = CryptographyManager.HexStringToByteArray(ciphertext);
        byte[] plaintextBytes = CryptographyManager.AesDecrypt(ciphertextBytes, certificate.GetMasterKey(), certificate.GetMasterIV());
        string plaintext = System.Text.Encoding.UTF8.GetString(plaintextBytes);

        //Verifica dell'ID dello Spider Robot nel ticket cifrato
        string spiderIDCheck = plaintext.Split(',')[0].Trim();
        if (spiderIDCheck != spiderManager.GetSpiderID())
        {
            return;
        }

        //Lettura del nonce generato dal Master Robot
        string masterNonce = plaintext.Split(',')[1].Trim();
        cacheKey.SetMasterNonce(masterNonce);

        //Calcolo della chiave di sessione
        CalculateSessionKey();

        //Invio del messaggio di autenticazione
        TimerCallback callback = state => SendAuthMessage();
        Timer timer = new Timer(callback, null, 1000, Timeout.Infinite);
    }

    private void CalculateSessionKey()
    {
        //Se non sono presenti i nonce, non è possibile calcolare la chiave di sessione
        if (cacheKey.GetMasterNonce() == null || cacheKey.GetSpiderNonce() == null)
        {
            return;
        }

        //Calcolo della chiave di sessione K_ss = SHA256(K_ms || IV_ms || N_ms || N_ss)
        byte[] messageToHash = spiderManager.GetCertificate().GetMasterKey();
        messageToHash = messageToHash.Concat(spiderManager.GetCertificate().GetMasterIV()).ToArray();
        messageToHash = messageToHash.Concat(System.Text.Encoding.UTF8.GetBytes(cacheKey.GetMasterNonce())).ToArray();
        messageToHash = messageToHash.Concat(System.Text.Encoding.UTF8.GetBytes(cacheKey.GetSpiderNonce())).ToArray();

        byte[] sessionKey = CryptographyManager.Sha256Hash(messageToHash);

        //Calcolo del vettore di inizializzazione IV_ss = SHA256(K_ms || IV_ms || N_ms || N_ss || K_ss)[0:16]
        messageToHash = messageToHash.Concat(sessionKey).ToArray();

        byte[] sessionIV = CryptographyManager.Sha256Hash(messageToHash).Take(16).ToArray();

        //Salvataggio della chiave di sessione
        cacheKey.SetSessionKey(sessionKey);
        cacheKey.SetSessionIV(sessionIV);
    }

    private void SendAuthMessage()
    {
        //ID dello Spider Robot
        string spiderID = spiderManager.GetSpiderID();

        //Preparazione del messaggio
        string message = $"Master, Auth, {spiderID}";

        //Nonce generato dal Master Robot
        string masterNonce = cacheKey.GetMasterNonce();

        //Cifratura del messaggio con la chiave di sessione
        string plaintext = $"{spiderID}, {masterNonce}";
        byte[] plaintextBytes = System.Text.Encoding.UTF8.GetBytes(plaintext);
        byte[] ciphertextBytes = CryptographyManager.AesEncrypt(plaintextBytes, cacheKey.GetSessionKey(), cacheKey.GetSessionIV());
        string ciphertext = CryptographyManager.ByteArrayToHexString(ciphertextBytes);

        message += ", " + ciphertext;

        //Invio del messaggio AUTH_SP
        EventManager.SendMessage(message);
    }

    private void ReceiveAuthMessage(string message)
    {
        //Se nel messaggio AUTH_MS non è presente l'ID dello Spider Robot, non è indirizzato a questo Spider Robot
        string spiderID = message.Split(',')[0].Trim();
        if (spiderID != spiderManager.GetSpiderID())
        {
            return;
        }

        //Decifratura del messaggio con la chiave di sessione
        string ciphertext = message.Split(',')[1].Trim();
        byte[] ciphertextBytes = CryptographyManager.HexStringToByteArray(ciphertext);
        byte[] plaintextBytes = CryptographyManager.AesDecrypt(ciphertextBytes, cacheKey.GetSessionKey(), cacheKey.GetSessionIV());
        string plaintext = System.Text.Encoding.UTF8.GetString(plaintextBytes);

        //Verifica dell'ID dello Spider Robot nel ticket cifrato
        string spiderIDCheck = plaintext.Split(',')[0].Trim();
        if (spiderIDCheck != spiderManager.GetSpiderID())
        {
            return;
        }

        //Verifica riguardante l'uguaglianzia tra il nonce nel ticket e quello generato dallo Spider Robot
        string spiderNonceCheck = plaintext.Split(',')[1].Trim();
        if (spiderNonceCheck != cacheKey.GetSpiderNonce())
        {
            return;
        }

        //Autenticazione completata
        cacheKey.SetIsAuth(true);
    }

    public void SendMineAlert(Vector3 position)
    {
        //Se non si è autenticati, non è possibile inviare messaggi
        if (!cacheKey.IsAuth())
        {
            return;
        }
        
        //Preparazione del messaggio
        String message = $"Master, MineAlert, {spiderManager.GetSpiderID()}";

        //Cifratura del messaggio con la chiave di sessione
        string plaintext = $"{(int)position.x}, {(int)position.y}, {(int)position.z}";
        byte[] plaintextBytes = System.Text.Encoding.UTF8.GetBytes(plaintext);
        byte[] ciphertextBytes = CryptographyManager.AesEncrypt(plaintextBytes, cacheKey.GetSessionKey(), cacheKey.GetSessionIV());
        string ciphertext = CryptographyManager.ByteArrayToHexString(ciphertextBytes);

        message += ", " + ciphertext;

        //Invio del messaggio MINE_SP
        EventManager.SendMessage(message);
    }
}
