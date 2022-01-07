# 3D-Graphics

Po prawej stronie ekranu są wszystkie kontrolki wymagane do zmiany trybu cieniowania lub pozycji kamery.
Poza tym interfejsem scena nie jest interaktywna.
Kierunek światła reflektora na ruchomym obiekcie jest zmieniany automatycznie w miarę ruchu obiektu.

W projekcie zaimplementowane są podstawowe elementy wymagane do renderowania grafiki 3D,
  wypełnianie wielokątów algorytmem scan-linii zoptymalizowanym do trójkątów,
  Phong shading (z mapowaniem wektorów normalnych z tekstury), Gouraud shading, flat shading, mgła,
  wczytywanie i teksturowanie modeli .obj, sceny z wieloma źródłami różnych, kolorowych świateł.
Poza biblioteką MathNet.Numerics używanej do operacji na macierzach i wektorach,
  nie są użyte żadne zewnętrzne biblioteki.
Vertex shadery i fragment shadery są implementowane przy pomcy klas implementujących IFragmentShader i IVertexShader,
  co umożliwia sporą elastyczność przy renderowaniu.
Używane są też dekoratory klas implementujących te interfejsy by móc np. dodać shader mgły do obiektów z np. shaderem Phonga.
