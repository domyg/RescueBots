using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Implementazione di una coda di priorità con heap binario
public class PriorityQueue<T> where T : IComparable<T>
{
    //Heap binario
    private T[] heap;
    //Numero di elementi nella coda
    private int size;
    //Capacità della coda
    private int capacity;

    //Costruttore di una coda di priorità con capacità iniziale pari a 10
    public PriorityQueue(int capacity = 10)
    {
        this.capacity = capacity;
        this.heap = new T[capacity];
        this.size = 0;
    }

    //Metodo per aggiungere un elemento alla coda
    public void Enqueue(T item)
    {
        //Se la coda è piena, raddoppia la capacità
        if (size == capacity)
        {
            Array.Resize(ref heap, capacity * 2);
            capacity *= 2;
        }

        int index = size;

        heap[size] = item;
        size++;

        //Sposta l'elemento nella posizione corretta
        while (index > 0 && heap[index].CompareTo(heap[Parent(index)]) < 0)
        {
            Swap(index, Parent(index));
            index = Parent(index);
        }
    }

    //Metodo per rimuovere un elemento dalla coda
    public T Dequeue()
    {
        //Se la coda è vuota, lancia un'eccezione
        if (size == 0)
        {
            throw new InvalidOperationException("Queue is empty");
        }

        //Prende l'elemento con priorità minima
        T item = heap[0];
        size--;

        heap[0] = heap[size];
        heap[size] = default(T);

        //Riordina la coda
        Heapify(0);

        return item;
    }

    //Metodo per prendere l'elemento con priorità minima senza rimuoverlo
    public T Peek()
    {
        if (size == 0)
        {
            throw new InvalidOperationException("Queue is empty");
        }

        return heap[0];
    }

    //Metodo per aggiornare la priorità di un elemento
    public bool UpdatePriority(T item)
    {
        for (int i = 0; i < size; i++)
        {
            //Se l'elemento è presente nella coda e la sua priorità è cambiata, lo sposta nella posizione corretta
            if (heap[i].Equals(item) && item.CompareTo(heap[i]) < 0)
            {
                heap[i] = item;
                Heapify(i);
                return true;
            }
        }

        return false;
    }

    //Metodo per ottenere il numero di elementi nella coda
    public int Count => size;

    //Metodo per sistemare un elemento nella posizione corretta
    private void Heapify(int index)
    {
        int left = Left(index);
        int right = Right(index);
        int smallest = index;

        if (left < size && heap[left].CompareTo(heap[smallest]) < 0)
        {
            smallest = left;
        }

        if (right < size && heap[right].CompareTo(heap[smallest]) < 0)
        {
            smallest = right;
        }

        if (smallest != index)
        {
            Swap(index, smallest);
            Heapify(smallest);
        }
    }

    //Metodo per scambiare due elementi
    private void Swap(int index1, int index2)
    {
        T temp = heap[index1];
        heap[index1] = heap[index2];
        heap[index2] = temp;
    }

    //Metodi per ottenere l'indice del genitore di un elemento
    private int Parent(int index)
    {
        return (index - 1) / 2;
    }

    //Metodi per ottenere l'indice del figlio sinistro di un elemento
    private int Left(int index)
    {
        return 2 * index + 1;
    }

    //Metodi per ottenere l'indice del figlio destro di un elemento
    private int Right(int index)
    {
        return 2 * index + 2;
    }
}
