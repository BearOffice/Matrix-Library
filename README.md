# Matrix-Library
This library can calculate matrix.  
Support lazy evaluation.  
The matrix obj can be loaded from/saved to plain text.  
<br>

# Usage (part)
Create a (2 * 3) matrix from 2D array.  
```
var matrix1 = new Matrix<string>(new[,]
{
    { "12g", "36a", "54c" },
    { "98f", "32d", "75e" }
});
Console.WriteLine(matrix1 + "\n");
```
>[["12g", "36a", "54c"]  
&nbsp;["98f", "32d", "75e"]]  

<br>Queries(extended methods) from class MathQ support lazy evaluation.  
.AsParallel() will let the remaining methods executed parallelly if needed.  
.AsParallel(force: true) will force the remaining methods executed parallely.  
```
var matrixseq = matrix1.AsParallel().Transpose().Map(str => int.Parse(str[0..^1]));
matrix1[0, 0] = "21b";

// Call .ToMatrix() will evaluate the sequence immediately.
var intmatrix1 = matrixseq.ToIntMatrix();
Console.WriteLine(intmatrix1 + "\n");
```
>[[21, 98]  
&nbsp;[36, 32]  
&nbsp;[54, 75]]

<br>Create a (3 * 2) blank int matrix and fill random numbers into it.
```
var intmatrix2 = new IntMatrix(3, 2);
var rand = new Random();
intmatrix2 = intmatrix2.Set((_, _) => rand.Next(10, 99)).ToIntMatrix();
Console.WriteLine(intmatrix2 + "\n");
```
>[[74, 93]  
&nbsp;[74, 22]  
&nbsp;[76, 82]]

<br>Concat the right matrix to the left matrix's right.
```
var intmatrix3 = intmatrix1 & intmatrix2;
Console.WriteLine(intmatrix3 + "\n");
```
>[[21, 98, 74, 93]  
&nbsp;[36, 32, 74, 22]  
&nbsp;[54, 75, 76, 82]]

<br>Create a (1 * 4) matrix from 1D array.
```
var intmatrix4 = new IntMatrix(new[] { 88, 77, 66, 55 });
Console.WriteLine(intmatrix4 + "\n");
```
>[[88, 77, 66, 55]]  

<br>Concat the right matrix to the left matrix's bottom.
```
var intmatrix5 = intmatrix3 | intmatrix4;
Console.WriteLine(intmatrix5 + "\n");
```
>[[21, 98, 74, 93]  
&nbsp;[36, 32, 74, 22]  
&nbsp;[54, 75, 76, 82]  
&nbsp;[88, 77, 66, 55]]  

<br>Create a matrix from string.  
```
var matrixstr = "[[\"sfe\", \"wrf\"]\n [\"rhj\", \"sgd\"]\n [\"dfg\", \"qac\"]]";
Console.WriteLine(matrixstr + "\n");

var matrix2 = Matrix.GetMatrixFromString<string>(matrixstr, new ParseFromString());
Console.WriteLine(matrix2 + "\n");
```
>[["sfe", "wrf"]  
&nbsp;["rhj", "sgd"]  
&nbsp;["dfg", "qac"]]<br><br>
[["sfe", "wrf"]  
&nbsp;["rhj", "sgd"]  
&nbsp;["dfg", "qac"]]  

<br>Modify the matrix's parse rule.
```
var zippedmatrix = intmatrix1.Zip(matrix2).ToMatrix();

var rule = new ParseToString();
rule.Add(typeof((int, string)), item =>
{
    var tuple = ((int, string))item;
    return $"<{tuple.Item1}-{tuple.Item2}>";
});
Console.WriteLine(zippedmatrix.ToString(rule));
```
>[["<21-sfe>", "<98-wrf>"]  
&nbsp;["<36-rhj>", "<32-sgd>"]  
&nbsp;["<54-dfg>", "<75-qac>"]]  
