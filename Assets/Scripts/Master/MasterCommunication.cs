using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;

public class MasterCommunication : MonoBehaviour
{
    private MasterManager masterManager;
    private PathFinder pathFinder;

    //Certificati delle chiavi di sessione per ogni Spider Robot
    private Dictionary<string, CacheKey> cacheKeys;

    void Start()
    {
        masterManager = GetComponent<MasterManager>();
        pathFinder = GetComponent<PathFinder>();

        cacheKeys = new Dictionary<string, CacheKey>();
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
        //Se il messaggio inizia con "Master, " è un messaggio per il Master
        if (message.StartsWith("Master, "))
        {
            message = message.Substring("Master, ".Length).Trim();
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
        if (message.StartsWith("MineAlert,"))
        {
            message = message.Substring("MineAlert,".Length).Trim();
            ReceiveMineAlert(message);
            return;
        }
    }

    private void ReceiveExchangeMessage(string message)
    {
        //ID dello Spider Robot che ha inviato il messaggio
        string spiderID = message.Split(',')[0].Trim();

        //Certificato della chiave Master dello Spider Robot
        Certificate certificate = masterManager.GetCertificate(spiderID);

        //Se non si è in possesso del certificato della chiave Master, non si può fare lo scambio
        if (certificate == null)
        {
            return;
        }

        //Cifratura del messaggio
        string ciphertext = message.Split(',')[1].Trim();
        byte[] ciphertextBytes = CryptographyManager.HexStringToByteArray(ciphertext);
        byte[] plaintextBytes = CryptographyManager.AesDecrypt(ciphertextBytes, certificate.GetMasterKey(), certificate.GetMasterIV());
        string plaintext = System.Text.Encoding.UTF8.GetString(plaintextBytes);

        //Verifica dell'ID dello Spider Robot
        string spiderIDCheck = plaintext.Split(',')[0].Trim();
        if (spiderIDCheck != spiderID)
        {
            return;
        }

        //Nonce generato dallo Spider Robot
        string spiderNonce = plaintext.Split(',')[1].Trim();
        if (cacheKeys.ContainsKey(spiderID))
        {
            cacheKeys.Remove(spiderID);
        }

        //Creazione del certificato e settaggio del nonce generato dallo Spider Robot
        cacheKeys.Add(spiderID, new CacheKey());
        cacheKeys[spiderID].SetSpiderNonce(spiderNonce);

        //Invio del messaggio di scambio di nonce
        TimerCallback callback = state => SendExchangeMessage(spiderID);
        Timer timer = new Timer(callback, null, 1000, Timeout.Infinite);
    }

    private void SendExchangeMessage(string spiderID)
    {
        //Certificato della chiave Master dello Spider Robot
        Certificate certificate = masterManager.GetCertificate(spiderID);

        //Se non si è in possesso del certificato della chiave Master, non si può fare lo scambio
        if (certificate == null)
        {
            return;
        }

        //Preparazione del messaggio
        string message = $"Spider, Exchange, {spiderID}";

        //Generazione del nonce dal Master Robot
        string masterNonce = CryptographyManager.GenerateRandomUUID();
        cacheKeys[spiderID].SetMasterNonce(masterNonce);

        //Cifratura del messaggio con la chiave Master
        string plaintext = $"{spiderID}, {masterNonce}";
        byte[] plaintextBytes = System.Text.Encoding.UTF8.GetBytes(plaintext);
        byte[] ciphertextBytes = CryptographyManager.AesEncrypt(plaintextBytes, certificate.GetMasterKey(), certificate.GetMasterIV());
        string ciphertext = CryptographyManager.ByteArrayToHexString(ciphertextBytes);

        //Aggiunta del messaggio cifrato al messaggio da inviare
        message += ", " + ciphertext;

        //Calcolo della chiave di sessione
        CalculateSessionKey(spiderID);

        //Invio del messaggio EXCH_MS
        EventManager.SendMessage(message);
    }

    private void CalculateSessionKey(string spiderID)
    {
        //Se non si è in possesso del certificato della chiave Master o degli nonce, non si può fare l'autenticazione
        if (!cacheKeys.ContainsKey(spiderID) || cacheKeys[spiderID].GetMasterNonce() == null || cacheKeys[spiderID].GetSpiderNonce() == null)
        {
            return;
        }

        //Calcolo della chiave di sessione K_ss = SHA256(K_ms || IV_ms || N_ms || N_ss)
        byte[] messageToHash = masterManager.GetCertificate(spiderID).GetMasterKey();
        messageToHash = messageToHash.Concat(masterManager.GetCertificate(spiderID).GetMasterIV()).ToArray();
        messageToHash = messageToHash.Concat(System.Text.Encoding.UTF8.GetBytes(cacheKeys[spiderID].GetMasterNonce())).ToArray();
        messageToHash = messageToHash.Concat(System.Text.Encoding.UTF8.GetBytes(cacheKeys[spiderID].GetSpiderNonce())).ToArray();

        byte[] sessionKey = CryptographyManager.Sha256Hash(messageToHash);

        //Calcolo dell'IV di sessione IV_ss = SHA256(K_ms || IV_ms || N_ms || N_ss || K_ss)[0:16]
        messageToHash = messageToHash.Concat(sessionKey).ToArray();

        byte[] sessionIV = CryptographyManager.Sha256Hash(messageToHash).Take(16).ToArray();

        //Salvataggio della chiave di sessione
        cacheKeys[spiderID].SetSessionKey(sessionKey);
        cacheKeys[spiderID].SetSessionIV(sessionIV);
    }

    private void ReceiveAuthMessage(string message)
    {
        //Se l'ID dello Spider Robot non è presente nel dizionario, non è possibile fare l'autenticazione
        string spiderID = message.Split(',')[0].Trim();
        if (!cacheKeys.ContainsKey(spiderID))
        {
            return;
        }

        //Cifratura del messaggio
        string ciphertext = message.Split(',')[1].Trim();
        byte[] ciphertextBytes = CryptographyManager.HexStringToByteArray(ciphertext);
        byte[] plaintextBytes = CryptographyManager.AesDecrypt(ciphertextBytes, cacheKeys[spiderID].GetSessionKey(), cacheKeys[spiderID].GetSessionIV());
        string plaintext = System.Text.Encoding.UTF8.GetString(plaintextBytes);

        //Verifica dell'ID dello Spider Robot
        string spiderIDCheck = plaintext.Split(',')[0].Trim();
        if (spiderIDCheck != spiderID)
        {
            return;
        }

        //Verifica del nonce generato dallo Spider Robot
        string masterNonceCheck = plaintext.Split(',')[1].Trim();
        if (masterNonceCheck != cacheKeys[spiderID].GetMasterNonce())
        {
            return;
        }

        //Invio del messaggio di autenticazione
        TimerCallback callback = state => SendAuthMessage(spiderID);
        Timer timer = new Timer(callback, null, 1000, Timeout.Infinite);
    }

    private void SendAuthMessage(string spiderID)
    {
        //Preparazione del messaggio
        string message = $"Spider, Auth, {spiderID}";

        string spiderNonce = cacheKeys[spiderID].GetSpiderNonce();

        //Cifratura del messaggio con la chiave di sessione
        string plaintext = $"{spiderID}, {spiderNonce}";
        byte[] plaintextBytes = System.Text.Encoding.UTF8.GetBytes(plaintext);
        byte[] ciphertextBytes = CryptographyManager.AesEncrypt(plaintextBytes, cacheKeys[spiderID].GetSessionKey(), cacheKeys[spiderID].GetSessionIV());
        string ciphertext = CryptographyManager.ByteArrayToHexString(ciphertextBytes);

        //Aggiunta del messaggio cifrato al messaggio da inviare
        message += ", " + ciphertext;

        //Autenticazione completata
        cacheKeys[spiderID].SetIsAuth(true);

        //Invio del messaggio AUTH_MS
        EventManager.SendMessage(message);
    }

    private void ReceiveMineAlert(string message)
    {
        //Se l'ID dello Spider Robot non è presente nel dizionario, non è possibile segnalare la mina
        string spiderID = message.Split(',')[0].Trim();
        if (!cacheKeys.ContainsKey(spiderID))
        {
            return;
        }

        //Cifratura del messaggio
        string ciphertext = message.Split(',')[1].Trim();
        byte[] ciphertextBytes = CryptographyManager.HexStringToByteArray(ciphertext);
        byte[] plaintextBytes = CryptographyManager.AesDecrypt(ciphertextBytes, cacheKeys[spiderID].GetSessionKey(), cacheKeys[spiderID].GetSessionIV());
        string plaintext = System.Text.Encoding.UTF8.GetString(plaintextBytes);

        //Lettura della posizione della mina
        int[] values = plaintext.Split(',').Select(int.Parse).ToArray();
        Assert.AreEqual(3, values.Length);

        //Segnalazione della mina
        Vector3 position = new Vector3(values[0], values[1], values[2]);
        pathFinder.ReportMine((int)position.x, (int)position.z);
    }
}
