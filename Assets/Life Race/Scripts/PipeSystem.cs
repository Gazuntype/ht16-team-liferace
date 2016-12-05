using UnityEngine;
using System.Collections;

public class PipeSystem : MonoBehaviour
{
    public Pipe pipePrefab;

    public int pipeCount = 6;
    public int emptyPipeCount = 2;

    public int eggDistance = 1000;

    Pipe[] pipes;
    int middle;
    bool spawnnedEgg = false;

    void Awake()
    {
        pipes = new Pipe[pipeCount];

        for (int i = 0; i < pipes.Length; i++)
        {
            Pipe pipe = pipes[i] = Instantiate(pipePrefab);
            pipe.transform.SetParent(transform, false);
        }
    }

    public Pipe SetupFirstPipe()
    {
        spawnnedEgg = false;
        middle = pipeCount / 2;

        for (int i = 0; i < pipes.Length; i++)
        {
            Pipe pipe = pipes[i];
            pipe.Generate(i > emptyPipeCount, false);

            if (i > 0)
            {
                pipe.AlignWith(pipes[i - 1]);
            }
        }

        AlignNextPipeWithOrigin();

        transform.localPosition = new Vector3(0f, -pipes[1].CurveRadius);
        return pipes[1];
    }

    public Pipe SetupNextPipe(bool at_finish)
    {
        ShiftPipes();
        AlignNextPipeWithOrigin();

        if (at_finish && !spawnnedEgg)
        {
            Debug.Log("Spawnned Egg");
            spawnnedEgg = true;
            pipes[pipes.Length - 1].Generate(false, true);
        }
        else
        {
            pipes[pipes.Length - 1].Generate(true, false);
        }
        
        pipes[pipes.Length - 1].AlignWith(pipes[pipes.Length - 2]);

        transform.localPosition = new Vector3(0f, -pipes[1].CurveRadius);
        return pipes[1];
    }

    void ShiftPipes()
    {
        Pipe pipe = pipes[0];

        for (int i = 1; i < pipes.Length; i++)
        {
            pipes[i - 1] = pipes[i];
        }

        pipes[pipes.Length - 1] = pipe;
    }

    void AlignNextPipeWithOrigin()
    {
        Transform transformToAlign = pipes[1].transform;

        for (int i = 0; i < pipes.Length; i++)
        {
            if (i != 1)
            {
                pipes[i].transform.SetParent(transformToAlign);
            }
        }

        transformToAlign.localPosition = Vector3.zero;
        transformToAlign.localRotation = Quaternion.identity;

        for (int i = 0; i < pipes.Length; i++)
        {
            if (i != 1)
            {
                pipes[i].transform.SetParent(transform);
            }
        }
    }


}
