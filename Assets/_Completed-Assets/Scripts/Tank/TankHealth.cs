using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class TankHealth : MonoBehaviour
    {
        public float m_StartingHealth = 100f;
        public Slider m_Slider;
        public Image m_FillImage;
        public Color m_FullHealthColor = Color.green;
        public Color m_ZeroHealthColor = Color.red;
        public GameObject m_ExplosionPrefab;

        private AudioSource m_ExplosionAudio;
        private ParticleSystem m_ExplosionParticles;
        private float m_CurrentHealth;
        private bool m_Dead;

        private bool m_CanRestoreHealth = true; // Flag to ensure health is restored only once per round.

        private void Awake()
        {
            m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
            m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
            m_ExplosionParticles.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            m_CurrentHealth = m_StartingHealth;
            m_Dead = false;
            SetHealthUI();
            m_CanRestoreHealth = true; // Reset health restoration flag at the beginning of the round.
        }

        public void TakeDamage(float amount)
        {
            m_CurrentHealth -= amount;
            SetHealthUI();

            if (m_CurrentHealth <= 0f && !m_Dead)
            {
                OnDeath();
            }
        }

        private void SetHealthUI()
        {
            m_Slider.value = m_CurrentHealth;
            m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
        }

        private void OnDeath()
        {
            m_Dead = true;
            m_ExplosionParticles.transform.position = transform.position;
            m_ExplosionParticles.gameObject.SetActive(true);
            m_ExplosionParticles.Play();
            m_ExplosionAudio.Play();
            gameObject.SetActive(false);
        }

        // Method to restore health for specific players on the specified keys
        private void Update()
        {
            // Get the TankMovement component
            TankMovement tankMovement = GetComponent<TankMovement>();

            if (tankMovement != null)
            {
                int playerNumber = tankMovement.m_PlayerNumber;

                // Check the input for each player (Blue tank - E, Red tank - Right Shift)
                if (playerNumber == 1 && Input.GetKeyDown(KeyCode.E) && m_CanRestoreHealth) // Blue tank
                {
                    RestoreHealth();
                }
                else if (playerNumber == 2 && Input.GetKeyDown(KeyCode.RightShift) && m_CanRestoreHealth) // Red tank
                {
                    RestoreHealth();
                }
            }
        }

        // Method to restore health
        public void RestoreHealth()
        {
            if (!m_Dead && m_CurrentHealth < m_StartingHealth)
            {
                m_CurrentHealth = m_StartingHealth; // Restore to full health
                SetHealthUI(); // Update the UI
                m_CanRestoreHealth = false; // Prevent further restoration during this round
            }
        }

        // Call this method at the end of each round to reset health restoration flag
        public void ResetHealthRestoration()
        {
            m_CanRestoreHealth = true; // Allow health restoration in the next round
        }
    }
}
