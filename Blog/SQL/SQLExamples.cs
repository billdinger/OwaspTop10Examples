using System.Data.SqlClient;
using Blog.Models;
using Microsoft.Extensions.Configuration;

namespace Blog.SQL
{
    public class SQLExamples
    {
        private IConfiguration Configuration { get; }

        public SQLExamples(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// A1 - Injection - This is an example of ADO.NET Raw SQL call in which we can't use entity framework for some reason, or
        /// a SPROC but we need to still execute SQl.
        /// </summary>
        /// <param name="comment">The comment to add to our database.</param>
        public void Add(Comment comment)
        {
            using (var conn = new SqlConnection(Configuration
                .GetConnectionString("CommentContext")))
            {

                var cmd = new SqlCommand("insert into comments Id,Body, Author Values ('@author', '@body', '@Id') ", conn);
                cmd.Parameters.Add(new SqlParameter(nameof(Comment.Author), comment.Author));
                cmd.Parameters.Add(new SqlParameter(nameof(Comment.Body), comment.Body));
                cmd.Parameters.Add(new SqlParameter(nameof(comment.Id), comment.Id));
                cmd.ExecuteNonQuery();
            }
        }
    }
}