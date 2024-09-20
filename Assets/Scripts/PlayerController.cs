using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Transform leftFootTarget; 
    public Transform rightFootTarget;
    public Transform[] leftFootPositions; 
    public Transform[] rightFootPositions;
    public Transform body; 
    public float stepDuration = 2f; 
    public float delayBeforeStart = 3f; 
    public float timeBetweenSteps = 30f; 
    public float rotationDuration = 1f; 

    private int currentStepIndex = 0;
    private bool isRightLegMoving = false;
    private bool isRotated = false; 
    private bool movementComplete = false; 

    private void Start()
    {
        StartCoroutine(StepRoutine());
    }

    IEnumerator StepRoutine()
    {
        yield return new WaitForSeconds(delayBeforeStart);

        while (!movementComplete)
        {
            if (!isRightLegMoving)
            {
                StartCoroutine(MoveLeg(rightFootTarget, rightFootPositions[currentStepIndex]));
                isRightLegMoving = true;
            }
            else
            {
                StartCoroutine(MoveLeg(leftFootTarget, leftFootPositions[currentStepIndex]));
                isRightLegMoving = false;

                currentStepIndex = (currentStepIndex + 1);

                if (currentStepIndex >= leftFootPositions.Length)
                {
                    movementComplete = true;
                    yield break;
                }
            }

            if (currentStepIndex == 5 || currentStepIndex == 10)
            {
                yield return StartCoroutine(RotateBody(90));
                isRotated = true;
            }
            else if (currentStepIndex == 7 || currentStepIndex == 11)
            {
                yield return StartCoroutine(RotateBody(0));
                isRotated = false;
            }

            yield return new WaitForSeconds(timeBetweenSteps);
        }
    }

    IEnumerator MoveLeg(Transform legTarget, Transform stepTarget)
    {
        Vector3 startPosition = legTarget.position;
        float elapsedTime = 0f;

        while (elapsedTime < stepDuration)
        {
            legTarget.position = Vector3.Lerp(startPosition, stepTarget.position, elapsedTime / stepDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        legTarget.position = stepTarget.position;
    }

    IEnumerator RotateBody(float targetYRotation)
    {
        Quaternion initialRotation = body.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, targetYRotation, 0);
        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            body.rotation = Quaternion.Lerp(initialRotation, targetRotation, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        body.rotation = targetRotation;
    }

    private void Update()
    {
        if (!movementComplete)
        {
            Vector3 averageFootPosition = (rightFootTarget.position + leftFootTarget.position) / 2;
            body.position = Vector3.Lerp(body.position, averageFootPosition, Time.deltaTime * 2f); 
        }
    }
}
