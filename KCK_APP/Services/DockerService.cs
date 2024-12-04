using Docker.DotNet;
using Docker.DotNet.Models;

namespace KCK_APP.Services
{
    public class DockerService
    {
        private const string ImageName = "postgres:latest";
        private const string ContainerName = "postgres";
        private const string Port = "5434";

        public async Task StartDockerContainerAsync()
        {
            using var client = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")).CreateClient();

            var containers = await client.Containers.ListContainersAsync(new ContainersListParameters() { All = true });
            var container = containers.FirstOrDefault(c => c.Names.Contains("/" + ContainerName));

            if (container == null)
            {
                var parameters = new CreateContainerParameters
                {
                    Image = ImageName,
                    Name = ContainerName,
                    ExposedPorts = new Dictionary<string, EmptyStruct>
                    {
                        { "5432/tcp", new EmptyStruct() }
                    },
                    HostConfig = new HostConfig
                    {
                        PortBindings = new Dictionary<string, IList<PortBinding>>
                        {
                            { "5432/tcp", new List<PortBinding> { new PortBinding { HostPort = Port } } }
                        }
                    }
                };

                var response = await client.Containers.CreateContainerAsync(parameters);
                await client.Containers.StartContainerAsync(response.ID, new ContainerStartParameters());
                //Console.WriteLine("Kontener PostgreSQL uruchomiony!");
            }
            else
            {
                //Console.WriteLine("Kontener już działa.");
            }
        }
    }
}