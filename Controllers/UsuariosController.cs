using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsuarioController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult Post(Usuario usuario)
    {
        try
        {
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return Ok(new
            {
                Message = "Usuario cadastrado com sucesso",
                Usuario = usuario
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Message = "Erro ao cadastrar usuario",
                Error = ex.Message
            });
        }
    }

    [HttpGet]
    public IActionResult Get()
    {
        var usuarios = _context.Usuarios.ToList();
        return Ok(usuarios);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var usuario = _context.Usuarios.Find(id);
        if (usuario == null)
            return NotFound(new { mensagem = "Usuário não encontrado." });

        return Ok(usuario);
    }

    // PUT: Atualizar usuário
    [HttpPut("{id}")]
    public IActionResult Put(int id, Usuario usuarioAtualizado)
    {
        var usuario = _context.Usuarios.Find(id);
        if (usuario == null)
            return NotFound(new { mensagem = "Usuário não encontrado." });

        usuario.Nome = usuarioAtualizado.Nome;
        usuario.Email = usuarioAtualizado.Email;
        usuario.Senha = usuarioAtualizado.Senha;

        _context.Usuarios.Update(usuario);
        _context.SaveChanges();

        return Ok(new { mensagem = "Usuário atualizado com sucesso!", usuario });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var usuario = _context.Usuarios.Find(id);
        if (usuario == null)
            return NotFound(new { mensagem = "Usuário não encontrado." });

        _context.Usuarios.Remove(usuario);
        _context.SaveChanges();

        return Ok(new { mensagem = "Usuário deletado com sucesso !" });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto login, [FromServices] IConfiguration configuration)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == login.Email && u.Senha == login.Senha);

        if (usuario == null)
            return Unauthorized("Usuário ou senha inválidos.");

        var token = TokenService.GenerateToken(usuario, configuration);
        return Ok(new { token });
    }

    public class LoginDto
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}
