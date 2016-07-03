# Serkit-RT-I
Another implementation for a circuit designer. Introducing Serkit script. Geometric designer. Circuit designer is non-functional.

Serkit Script

! Marked Fields are necessary

!Serkit <name>						- To define a new circuit component
Scale								- To set the scale at which the component is displayed
!Geometry							- For displaying the component the geometry definition is necessary
	Line "sw sc" x1 y1 x2 y2		- sw : Stroke Width
									  sc : Stroke Color in Hex format (#aarrggbb)
									  x1, y1, x2, y2 : The co-ordinates of the two end points
	Polygon "sw sc" <x y>			- sw : Stroke Width
								      sc : Stroke Color in Hex format (#aarrggbb)
									  <x y> : Points to join separated by spaces
	FillPolygon "sw sc fc" <x y>	- sw : Stroke Width
									  sc : Stroke Color in Hex format (#aarrggbb)
									  fc : Fill Color in Hex format (#aarrggbb)
									  <x y> : Points to join separated by spaces
	Ellipse "sw sc" x y r			- sw : Stroke Width
								      sc : Stroke Color in Hex format (#aarrggbb)
									  x y : Centre
									  r : Radius
	FillEllipse "sw sc fc" x y r	- sw : Stroke Width
								      sc : Stroke Color in Hex format (#aarrggbb)
									  fc : Fill Color in Hex format (#aarrggbb)
									  x y : Centre
									  r : Radius
	Text "<text>" size x y			- <text> : Text to be displayed
									  size : Font Size
									  x y : Location
!EndGeometry						- To close the geometry definition block
!IO	(i o) "<names>"					- For defining the behavior of the circuit
									  i : Input Count
									  o : Output Count
									  <names> : The names of the inputs and outputs in order
	<name x y>						- Set location of all inputs/outputs
									  name : The previously defined name for an input/output
									  x y : The location co-ordinates for the input/output
	Behavior can be specified as per the Table provided in SerkitOperators file.
!EndIO
!EndSerkit
