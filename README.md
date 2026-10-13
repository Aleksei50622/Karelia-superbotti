# Karelia-superbotti
Karelia-superbotti projekti

NetCordiin pohjautuva Discord botti joka käyttää LocalAI:ta erilaisiin komentoihin

Käyttö vaatii seuraavaa:
<ol>
<li>Discord botin tokeni määritetty appsettings.json tiedostossa</li>
<li>LocalAI kontti päällä määritetyssä hostissa</li>
<li>Chatti ja TTS modelit valmiina LocalAI:ssa. Tällä hetkellä määrittely suoraan Services/Models.cs </li>
</ol>

Botilla on seuraavat kommennot:
<ol>
  <li>/summary - Tiivistää chat keskustelun ja antaa sen myös äänimuodossa</li>
  <li>/speak - Muuttaa edellisen viestin äänimuotoon</li>
  <li>/models - Modelin vaihtoa varten</li>
</ol>
