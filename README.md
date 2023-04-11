Aufgaben:

1. Offne https://auth.mlu.dev.neo.onl/

2. Erstelle einen Client mit dem Namen: test
   - Client authentication: On
   - Standard flow: On
   - Direct access grants: On
   - Implicit flow: On

3. Trage die entsprechenden Informationen in der ".env" Datei ein.

----

4. Erstelle eine Realm Rolle: realmtest

5. Erstelle eine Client Rolle in test: clienttest

6. Erstelle einen Benutzer: tester

7. Weise dem Benutzer das Kennwort zu

8. Weise dem Benutzer die Rollen realmtest und clienttest zu

----

9. Kompiliere die Programme mit make

10. Hole ein token mit ./password-grant username passwort

11. Dekodiere das token mit ./decode-token idToken.txt

----

12. Hole ein token mit ./authorization-code-flow

----

13. Erstelle einen Mapper, der die Client Rollen in den Token einblendet.