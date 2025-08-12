using UnityEngine;

namespace Engine.Assets.Engine.Scripts.Timelines.NotesDestroy
{
	public class NotePartsDestroy : MonoBehaviour
	{
		private float _speed;
		private Color _color = Color.white;
		private NotePartsMesh _mesh;

		public NotePartsMesh Mesh => _mesh;

		public void SetMesh(NotePartsMesh mesh)
		{
			_mesh = mesh;
		}

		public void Play(float speed = 3)
		{
			_speed = speed;
		}

		private void Update()
		{

		}
	}
}
