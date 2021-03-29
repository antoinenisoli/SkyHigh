using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CrowdMember : MonoBehaviour
{
    [SerializeField] Color[] colors;
    [Tooltip("How fast this crowd member should move, in units per second.")]
    [SerializeField] float moveSpeed = 1f;
    [SerializeField] float rotSpeed = 50f;
    [Tooltip("How long it takes in seconds for this member to fade in/out when starting/coming to the end of its route.")]
    [SerializeField] float fadeDuration;

    [Tooltip("The path this crowd member is following.\n" +
        "Elements are connected by a straight line.\n" +
        "[0] = start, [length - 1] = end.")]
    [SerializeField] Transform[] route;

    Renderer rend;
    int nextRouteIndex = 1;
    bool endActionInvoked = false;
    Material fadeMat;
    bool canFadeInUpdate = true;
    Transform currentTarget;

    private void Start() 
    {
        rend = GetComponentInChildren<Renderer>();
        fadeMat = rend.material;
        fadeMat.SetColor("_Color", colors[Random.Range(0, colors.Length)]);
    }

    private void Update()
    {
        //If the route is valid and we're not at the end,
        if (route.Length > 1 && nextRouteIndex < route.Length)
        {
            //Move towards the next waypoint in the route at `speed` units per second.
            transform.position = Vector3.MoveTowards(transform.position, route[nextRouteIndex].position, moveSpeed * Time.deltaTime);
            currentTarget = route[nextRouteIndex];
            Vector3 distance = currentTarget.position - transform.position;
            distance += Vector3.right * 0.001f;
            Quaternion quater = Quaternion.LookRotation(distance.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, quater, rotSpeed * Time.deltaTime);

            //If this member is approximately `fadeDuration` seconds away from reaching the final point in its route, start fading out
            //  (Since the member moves at `speed` units per second, it will reach the end point in about `fadeDuration` seconds 
            //  when it is (speed * fadeDuration) units away from the end point.)
            if (canFadeInUpdate
                && nextRouteIndex >= route.Length - 1
                && (transform.position - route[nextRouteIndex].position).sqrMagnitude <= Mathf.Pow(moveSpeed * fadeDuration, 2))
            {
                canFadeInUpdate = false;
                StopAllCoroutines();
                StartCoroutine(FadeInOut(false, fadeDuration));
            }

            if (transform.position == route[nextRouteIndex].position) 
                nextRouteIndex++; 
        }
        else if (!endActionInvoked)
        {
            EventManager.Instance.onCrowdMemberReachedEnd?.Invoke(this);
            endActionInvoked = true;
        }
    }

    /// <summary>
    /// Gives this crowd member a new route, places this member at the start of that route, and resets relevant internal vars.
    /// </summary>
    /// <param name="newRoute">This crowd member's new route.</param>
    public void ResetRoute(params Transform[] newRoute)
    {
        //If the route is invalid, bail out and cry about it in the console.
        if (newRoute.Length < 2)
        {
            Debug.LogError($"Invalid route assigned to {gameObject.name}. The route length must be at least 2, not {newRoute.Length}.");
            return;
        }

        nextRouteIndex = 1;
        route = newRoute;
        transform.position = newRoute[0].position;

        canFadeInUpdate = true;
        endActionInvoked = false;

        StopAllCoroutines();
        StartCoroutine(FadeInOut(true, fadeDuration));
    }

    private IEnumerator FadeInOut(bool fadingIn, float duration)
    {
        float alpha = fadingIn ? 1 : 0;
        float progress = 0;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(DOTween.To(() => progress, x => progress = x, alpha, duration));
        sequence.Play();
        yield return null;

        while (sequence.IsPlaying())
        {
            fadeMat.SetFloat("_Opacity", progress);
            yield return null;
        }

        /*
        //If fading in, go from 0 to 1, and vice versa.
        float startAlpha = fadingIn ? 0 : 1;
        float endAlpha = fadingIn ? 1 : 0;
        float progress = 0;
        DOTween.To(() => progress, x => progress = x, startAlpha, duration);
        fadeMat.SetFloat("_Opacity", progress);
        yield return null;
        //While still fading,
        while (progress < 1)
        {
            //Advance one step such that progress = 1 once `duration` seconds have passed
            progress += Time.deltaTime / duration;
            //Lerp the renderer's alpha from start to end with easing at the start and end
            float newAlpha = Mathf.SmoothStep(startAlpha, endAlpha, progress);
            yield return null;
        }*/
    }
}
