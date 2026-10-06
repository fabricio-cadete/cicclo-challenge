# Infrastructure

Implementará os contratos definidos nas camadas internas para persistência e integrações externas. Referencia Application e Domain. Por ora os dados ficam em memória (`InMemoryLaundryServiceRepository`, singleton), sem banco de dados; reiniciar a API recria os serviços.
