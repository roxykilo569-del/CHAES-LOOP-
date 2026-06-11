using UnityEngine;
namespace SciFiEnvironments.ScifiFX
{
	public class UVAnim_Trail : MonoBehaviour
	{

		public enum AnimState
		{
			xAxis, yAxis
		}
		public AnimState m_animState;
		public float speed;

		public void Init(AnimState type, float _speed)
		{
			m_animState = type;
			speed = _speed;
		}
		void Start()
		{
		}
		void Update()
		{
			print(transform.GetComponent<ParticleSystemRenderer>().trailMaterial);
			if (m_animState == AnimState.xAxis)
			{
				
				transform.GetComponent<ParticleSystemRenderer>().trailMaterial.mainTextureOffset += new Vector2(speed * Time.deltaTime, 0);
			}
			if (m_animState == AnimState.yAxis)
			{
				transform.GetComponent<ParticleSystemRenderer>().trailMaterial.mainTextureOffset += new Vector2(0, speed * Time.deltaTime);
			}
		}
	}
}