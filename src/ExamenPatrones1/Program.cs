using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamenPatrones1
{
    internal class Program
    {
        public class Banco
        {
            private static Banco _instance;
            private static readonly object _lock = new object();
            //Tipos de tarnsacciones que ofrece el banco
            public string Retiro { get; private set; } // Tipo de transacción: Retiro
            public string Deposito { get; private set; } // Tipo de transacción: Deposito
            public string Nomina { get; private set; } // Tipo de transacción: Nomina
            public string Transferencia { get; private set; } // Tipo de transacción: Transferencia

            private Banco()
            {
                Retiro = "Retiro";
                Deposito = "Deposito";
                Nomina = "Nomina";
                Transferencia = "Transferencia";
            }
            public void Mostrar()
            {
                Console.WriteLine($"Tipos de transferencias:\n1.-{Retiro}\n2.-{Deposito}\n3.-{Nomina}\n4.-{Transferencia}");

            }

            public static Banco Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        lock (_lock)
                        {
                            if (_instance == null)
                                _instance = new Banco();
                        }
                    }
                    return _instance;
                }
            }
        }
        public class Transacciones
        {
            public int Id { get; set; }
            public bool Activa { get; set; }
            public string Tipo { get; set; }

            public void Iniciar()
            {
                if (Activa)
                {
                    Console.WriteLine("La transacción no ha terminado o está en uso");
                }
                else
                {
                    Activa = true;
                    Console.WriteLine($"Transacción con: {Id}. ¡Activa!");
                }
            }
            public void Terminar()
            {
                Activa = false;
                Console.WriteLine($"La transacción con el Id: {Id}. Ha terminado");
            }
        }

        // Implementación genérica del Object Pool
        public class ObjectPool<T> where T : Transacciones, new()
        {
            private readonly Stack<T> _pool = new Stack<T>();
            private int _contador = 0;
            private const int MAX_CAPACIDAD = 10;

            public T Obtener(string tipo)
            {
                if (_pool.Count > 0)
                {
                    var obj = _pool.Pop();
                    Console.WriteLine($"Reutilizando transacción con Id: {obj.Id}");
                    obj.Activa = true;
                    obj.Tipo = tipo;
                    return obj;
                }

                // Si no hay objetos disponibles en el pool
                if (_contador < MAX_CAPACIDAD)
                {
                    var nuevo = new T { Id = ++_contador, Activa = true, Tipo = tipo };
                    Console.WriteLine($"Creando nueva transacción con Id: {nuevo.Id}");
                    return nuevo;
                }

                // Límite alcanzado y ningún objeto libre
                Console.WriteLine("No hay recursos disponibles. Por favor, termine una transacción antes de iniciar otra.");
                return null;
            }

            public void Devolver(T obj)
            {
                if (obj.Activa)
                {
                    Console.WriteLine("La transacción sigue activa. No puede devolverse.");
                }
                else
                {
                    Console.WriteLine($"Transacción con Id: {obj.Id} devuelta al pool.");
                    _pool.Push(obj);
                }
            }

            public int Disponible => _pool.Count;
        }



        static void Main(string[] args)
        {
            // Singleton: Banco
            Banco banco1 = Banco.Instance;
            Banco banco2 = Banco.Instance;
            Console.WriteLine("Revisando que los bancos sean iguales: " + ReferenceEquals(banco1, banco2));

            // Object Pool con capacidad finita
            var pool = new ObjectPool<Transacciones>();

            List<Transacciones> usuarioTransacciones = new List<Transacciones>();
            char continuarProceso;
            do
            {
                char continuar = ' ';
                int opcion = 0;

                //=== Parte 1: Crear nuevas transacciones usando Object Pool ===
                do
                {
                    banco1.Mostrar();
                    Console.Write("¿Cuál opción va a escoger? ");
                    if (!int.TryParse(Console.ReadLine(), out opcion))
                    {
                        Console.WriteLine("Opción inválida. Intente de nuevo.");
                        continue;
                    }
                    string tipo;
                    switch (opcion)
                    {
                        case 1: tipo = "Retiro"; break;
                        case 2: tipo = "Deposito"; break;
                        case 3: tipo = "Nomina"; break;
                        case 4: tipo = "Transferencia"; break;
                        default: tipo = null; break;
                    }

                    if (tipo == null)
                    {
                        Console.WriteLine("No se maneja este tipo de transacción.");
                    }
                    else
                    {
                        var transaccion = pool.Obtener(tipo);
                        if (transaccion != null)
                        {
                            usuarioTransacciones.Add(transaccion);
                        }
                        else
                        {
                            //Si el Pool está vacío pasar a la sección de finalizar transacciones
                            break;
                        }
                    }

                    Console.WriteLine($"\nPool disponible: {pool.Disponible}");
                    Console.WriteLine("¿Va a realizar más transacciones? s/n");
                    continuar = char.Parse(Console.ReadLine().ToLower());
                    Console.WriteLine();

                } while (continuar != 'n');

                //=== Parte 2: Finalizar transacciones activas ===
                char continuarFinalizar = ' ';
                do
                {
                    var activas = usuarioTransacciones.Where(t => t.Activa).ToList();

                    if (activas.Count == 0)
                    {
                        Console.WriteLine("No hay transacciones por finalizar.");
                        break;
                    }

                    Console.WriteLine("Transacciones activas:");
                    foreach (var t in activas)
                    {
                        Console.WriteLine($"Id: {t.Id} - Tipo: {t.Tipo}");
                    }

                    Console.Write("Ingrese Id de la transacción a finalizar: ");

                    if (int.TryParse(Console.ReadLine(), out int idFinalizar)) //Obliga a que sea un número
                    {
                        var trans = usuarioTransacciones.FirstOrDefault(t => t.Id == idFinalizar && t.Activa);
                        if (trans != null)
                        {
                            trans.Activa = false;
                            Console.WriteLine($"Transacción Id {trans.Id} finalizada.");
                        }
                        else
                        {
                            Console.WriteLine("Id inválido o transacción ya finalizada.");
                        }
                    }

                    Console.WriteLine("¿Desea finalizar otra transacción? s/n");
                    continuarFinalizar = char.Parse(Console.ReadLine().ToLower());
                    Console.WriteLine();

                } while (continuarFinalizar != 'n');

                //=== Parte 3: Devolver transacciones terminadas al pool ===
                Console.WriteLine("Devolviendo transacciones finalizadas al pool...");
                for (int i = usuarioTransacciones.Count - 1; i >= 0; i--)
                {
                    if (!usuarioTransacciones[i].Activa)
                    {
                        pool.Devolver(usuarioTransacciones[i]);
                        usuarioTransacciones.RemoveAt(i);
                    }
                }

                Console.WriteLine($"\nPool disponible: {pool.Disponible}");
                Console.WriteLine("¿Desea repetir el proceso? s/n");
                continuarProceso = char.Parse(Console.ReadLine().ToLower());
                Console.WriteLine();

            } while (continuarProceso != 'n');

            Console.WriteLine("Programa finalizado.");
        }
    }
}
