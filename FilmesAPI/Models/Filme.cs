﻿using System.ComponentModel.DataAnnotations;

namespace FilmesAPI.Models;

public class Filme
{
    [Key]
    [Required]
    public int Id { get; set; }
    [Required(ErrorMessage = "O titulo do filme é obrigatório")]
    public string Titulo { get; set; }
    [Required(ErrorMessage = "O Genero do filme é obrigatório")]
    [MaxLength(50, ErrorMessage = "O tamanho do genoro não deve exceder 50 caracteres")]
    public string Genero { get; set; }
    [Required]
    [Range(70, 600,ErrorMessage = "A duração de ter entre 70 e 600 minutos")]
    public int Duracao { get; set; }
    
}
