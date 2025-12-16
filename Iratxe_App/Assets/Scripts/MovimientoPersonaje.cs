using UnityEngine;

public class MovimientoPersonaje : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private Vector3[] puntosDeMovimiento; // Puntos donde el personaje puede moverse
    [SerializeField] private float velocidad = 2f; // Velocidad de movimiento
    [SerializeField] private float distanciaMinima = 0.1f; // Distancia para considerar que llegó al punto
    
    [Header("Configuración de Animación")]
    [SerializeField] private Animator animator; // Referencia al Animator
    [SerializeField] private string parametroMovimiento = "isMoving"; // Nombre del parámetro bool en el Animator
    
    private int indiceObjetivo; // Índice del punto objetivo actual
    private Vector3 puntoObjetivo; // Posición objetivo actual
    
    void Start()
    {
        // Si no se asignó el Animator, intentar obtenerlo del GameObject
        if (animator == null)
            animator = GetComponent<Animator>();
        
        // Validar que tengamos puntos de movimiento
        if (puntosDeMovimiento == null || puntosDeMovimiento.Length == 0)
        {
            Debug.LogWarning("No se han definido puntos de movimiento. Desactivando script.");
            enabled = false;
            return;
        }
        
        // Seleccionar un punto aleatorio inicial
        SeleccionarNuevoPuntoAleatorio();
    }
    
    void Update()
    {
        MoverHaciaPunto();
    }
    
    void MoverHaciaPunto()
    {
        // Calcular la distancia al objetivo
        float distancia = Vector3.Distance(transform.position, puntoObjetivo);
        
        // Si llegamos al punto, seleccionar uno nuevo
        if (distancia <= distanciaMinima)
        {
            SeleccionarNuevoPuntoAleatorio();
            return;
        }
        
        // Activar animación de movimiento
        if (animator != null)
            animator.SetBool(parametroMovimiento, true);
        
        // Mover hacia el punto objetivo
        Vector3 direccion = (puntoObjetivo - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, puntoObjetivo, velocidad * Time.deltaTime);
        
        // Rotar el personaje hacia la dirección de movimiento
        if (direccion != Vector3.zero)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * 5f);
        }
    }
    
    void SeleccionarNuevoPuntoAleatorio()
    {
        // Seleccionar un índice aleatorio diferente al actual
        int nuevoIndice;
        do
        {
            nuevoIndice = Random.Range(0, puntosDeMovimiento.Length);
        }
        while (nuevoIndice == indiceObjetivo && puntosDeMovimiento.Length > 1);
        
        indiceObjetivo = nuevoIndice;
        puntoObjetivo = puntosDeMovimiento[indiceObjetivo];
    }
    
    // Método para visualizar los puntos en el editor
    private void OnDrawGizmos()
    {
        if (puntosDeMovimiento == null || puntosDeMovimiento.Length == 0)
            return;
        
        Gizmos.color = Color.cyan;
        foreach (Vector3 punto in puntosDeMovimiento)
        {
            Gizmos.DrawWireSphere(punto, 0.3f);
        }
        
        // Mostrar líneas entre los puntos
        Gizmos.color = Color.yellow;
        for (int i = 0; i < puntosDeMovimiento.Length - 1; i++)
        {
            Gizmos.DrawLine(puntosDeMovimiento[i], puntosDeMovimiento[i + 1]);
        }
    }
}
