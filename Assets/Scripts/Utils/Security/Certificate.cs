using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Classe che rappresenta un certificato contenente la chiave Master di cifratura
//sono presenti solo metodi Get per evitare che il certificato venga modificato
public class Certificate
{
    private string spiderID;
    private byte[] masterKey;
    private byte[] masterIV;

    public Certificate(string spiderID, byte[] masterKey, byte[] masterIV)
    {
        this.spiderID = spiderID;
        this.masterKey = masterKey;
        this.masterIV = masterIV;
    }

    public string GetSpiderID()
    {
        return spiderID;
    }

    public byte[] GetMasterKey()
    {
        return masterKey;
    }

    public byte[] GetMasterIV()
    {
        return masterIV;
    }
}
