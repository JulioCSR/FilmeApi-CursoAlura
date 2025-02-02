using AutoMapper;
using FilmesAPI.Data;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class FilmeController : ControllerBase
{
    private FilmeContext _context;
    private IMapper _mapper;

    public FilmeController(FilmeContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    /// <summary>
    /// Adiciona um filme ao banco de dados
    /// </summary>
    /// <param name="filmeDto">Objeto com os campos necessários para criação de um filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso inserção seja feita com sucesso</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult AdicionaFilme([FromBody] CreateFilmeDto filmeDto)
    {
        Filme filme = _mapper.Map<Filme>(filmeDto);
        _context.Filmes.Add(filme);
        _context.SaveChanges();
        return CreatedAtAction(nameof(RecuperaFilmePorId), new { id = filme.Id }, filme);

    }

    /// <summary>
    /// Recupera uma lista paginada de filmes do banco de dados.
    /// </summary>
    /// <param name="skip">Número de registros a serem ignorados antes de começar a retornar os resultados.</param>
    /// <param name="take">Número máximo de registros a serem retornados.</param>
    /// <returns>Lista de filmes no formato ReadFilmeDto.</returns>
    /// <response code="200">Retorna a lista de filmes recuperada com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]

    public IEnumerable<ReadFilmeDto> RecuperaFilmes([FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        return _mapper.Map<List<ReadFilmeDto>>(_context.Filmes.Skip(skip).Take(take));
    }

    /// <summary>
    /// Recupera um Filme por ID do banco de dados.
    /// </summary>
    /// /// <param name="id">ID do filme para recuperação.</param>
    /// /// <returns>Filme do id recuperado</returns>
    /// <response code="200">Caso a recuperação seja feita com sucesso</response>
    [HttpGet("{Id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult RecuperaFilmePorId(int id)
    {
        //para retorna 404 quando não encontrar algum recurso
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null)
        {
            //NotFound para quando não encontrar
            return NotFound();
        }
        else
        {
            //retorna o resultado
            var filmeDto = _mapper.Map<ReadFilmeDto>(filme);
            return Ok(filmeDto);
        }
    }

    /// <summary>
    /// Atualiza todos os campos de um filme existente no banco de dados com base no ID informado.
    /// </summary>
    /// <param name="id">Identificador único do filme a ser atualizado.</param>    
    /// <returns>Retorna um status HTTP indicando o resultado da operação.</returns>
    /// <response code="204">Filme atualizado com sucesso.</response>
    /// <response code="404">Filme não encontrado.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult AtualizaFilme(int id, [FromBody] UpdateFilmeDto filmeDto)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null)
        {
            return NotFound();
        }
        else
        {
            _mapper.Map(filmeDto, filme);
            _context.SaveChanges();
            return NoContent();
        }

    }

    /// <summary>
    /// Realiza uma atualização parcial nos campos de um filme existente com base no ID informado.
    /// </summary>
    /// <param name="id">Identificador único do filme a ser atualizado.</param>
    /// <param name="patch">Objeto contendo as operações de atualização parcial a serem aplicadas ao filme.</param>
    /// <returns>Retorna um status HTTP indicando o resultado da operação.</returns>
    /// <response code="204">Filme atualizado com sucesso.</response>
    /// <response code="400">Requisição inválida ou erro na validação dos dados.</response>
    /// <response code="404">Filme não encontrado.</response>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult AtualizaFilmeParcial(int id, JsonPatchDocument<UpdateFilmeDto> patch)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);

        if (filme == null)
        {
            return NotFound();
        }
        var filmeParaAtualizar = _mapper.Map<UpdateFilmeDto>(filme);
        patch.ApplyTo(filmeParaAtualizar, ModelState);

        if (!TryValidateModel(filmeParaAtualizar))
        {
            return ValidationProblem(ModelState);
        }
        else
        {

            _mapper.Map(filmeParaAtualizar, filme);
            _context.SaveChanges();
            return NoContent();
        }

    }

    /// <summary>
    /// Remove um filme do banco de dados com base no ID informado.
    /// </summary>
    /// <param name="id">Identificador único do filme a ser removido.</param>
    /// <returns>Retorna um status HTTP indicando o resultado da operação.</returns>
    /// <response code="204">Filme removido com sucesso.</response>
    /// <response code="404">Filme não encontrado.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeletaFilme(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);

        if (filme == null)
        {
            return NotFound();
        }
        else
        {
            _context.Remove(filme);
            _context.SaveChanges();
            return NoContent();
        }

    }
}
