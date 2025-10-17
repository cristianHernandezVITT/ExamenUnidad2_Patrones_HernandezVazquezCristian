# ExamenUnidad2_Patrones_HernandezVazquezCristian
**Nombre**:
Hernández Vazquez Cristian

**Nombre del proyecto**:
Operaciones Bancarias

**Breve descripción** :
Programa que usa los patrones de diseño singleton y Object Pool para simular operaciones bancarias.
A través de singleton se crea una clase de banco central con sus campos definidos.
Con object pool se crean máximo 10 objetos para simular las transacciones bancarias.

Primero:

El programa va a solicitar que cree algunas transacciones bancarias (retiro, deposito, transferencia y nomina), para ir a la siguiente fase si considera que ha terminado correctamente escriba s o n. Nota importante, sólo va a poder crear máximo 10, si se pasa va a ir a la siguiente parte automáticamente. No es necesario hacer los 10 objetos.

Segundo:

Una vez creado los tipos de transacciones, el programa va a solicitar que los termine. No es necesario terminarlos todos. Si los termina todos. Pasará a la siguiente parte.

Tercero:

Una vez indicado cuáles programas van a terminar, el programa automáticamente va a seleccioanr los programas que usted quiso y los va a devolver al object pool. Si es la primera vez, la mayor parte del tiempo va a marcar que hay 0 en el object pool, después de la primera vez se espera que siempre haya al menos un valor. Por supuesto, la cantidad de objetos en el object pool va a cambair las primeras veces, si desde el inicio se llena (se crean 10 objetos) no va a cambiar. 
