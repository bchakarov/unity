using UnityEngine;

public class BackgroundCollideController : MonoBehaviour
{
    private int numberOfBackgrouds;
    private float distanceBetweenBackgrounds;

    private int numberOfGrounds;
    private float distanceBetweenGrounds;

    private int numberOfPipes;
    private float distanceBetweenPipes;

    private bool upperPipe;

    public void Start()
    {
        var backgrounds = GameObject.FindGameObjectsWithTag("Background");
        var grounds = GameObject.FindGameObjectsWithTag("Ground");
        var pipes = GameObject.FindGameObjectsWithTag("Pipe");
        
        this.RandomizePipes(pipes);

        this.numberOfBackgrouds = backgrounds.Length;
        this.numberOfGrounds = grounds.Length;
        this.numberOfPipes = pipes.Length;

        if (this.numberOfBackgrouds < 2 || this.numberOfGrounds < 2 || this.numberOfPipes < 2)
        {
            throw new System.InvalidOperationException("You must have at least two backgrounds or grounds or pipes in your scene!");
        }

        this.distanceBetweenBackgrounds = this.DistanceBetweenObjects(backgrounds[0], backgrounds[1]);
        this.distanceBetweenGrounds = this.DistanceBetweenObjects(grounds[0], grounds[1]);
        this.distanceBetweenPipes = this.DistanceBetweenObjects(pipes[0], pipes[1]);
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Background") 
            || collider.CompareTag("Ground") 
            || collider.CompareTag("Pipe"))
        {
            var go = collider.gameObject;
            var originalPosition = go.transform.position;
            
            if (collider.CompareTag("Background"))
            {
                originalPosition.x += this.numberOfBackgrouds * this.distanceBetweenBackgrounds;
            }
            else if (collider.CompareTag("Ground"))
            {
                originalPosition.x += this.numberOfGrounds * this.distanceBetweenGrounds;
            }
            else
            {
                originalPosition.x += this.numberOfPipes * this.distanceBetweenPipes;

                var randomY = 0f;
                if (this.upperPipe)
                {
                    // generate upper pipe
                    randomY = Random.Range(0, 2);
                }
                else
                {
                    // generate down pipe
                    randomY = Random.Range(-2, 0);
                }

                originalPosition.y = randomY;

                this.upperPipe = !this.upperPipe;
            }

            go.transform.position = originalPosition;
        }
    }

    private float DistanceBetweenObjects(GameObject first, GameObject second)
    {
        return Mathf.Abs(first.transform.position.x - second.transform.position.x);
    }

    private void RandomizePipes(GameObject[] pipes)
    {
        int count = 0;

        for (int i = 1; i < pipes.Length; i++)
        {
            count++;
            var currentPipe = pipes[i];
            var randomY = 0f;
            if (count % 2 == 0) // upper pipe
            {
                randomY = Random.Range(0, 2);
            }
            else // down pipe
            {
                randomY = Random.Range(-2, 0);
            }

            var pipePosition = currentPipe.transform.position;
            pipePosition.y = randomY;
            currentPipe.transform.position = pipePosition;
        }
    }
}
