# Monitoring with Grafana & Loki

This project uses Grafana and Loki for log storage and visualization. Promtail collects container logs and forwards them to Loki. Datadog has been removed from the compose stack.

## Services in docker-compose
- grafana — UI on port 3000
- loki — log storage on port 3100
- promtail — log collector

## Quick start

1. From the project directory run:

```bash
docker-compose up -d --build
```

2. Open Grafana at http://localhost:3000 (default admin/admin).

3. Add Loki as a data source in Grafana:
   - URL: http://loki:3100
   - Type: Loki

4. Use Explore to query logs or build dashboards.

## Promtail configuration
- promtail-config.yaml is included at the repository root and mounted into the promtail container. It scrapes Docker container logs under /var/lib/docker/containers and /var/log.

## Troubleshooting
- If Grafana cannot reach Loki, confirm both containers are running and on the same Docker network.
- Promtail needs read access to /var/lib/docker/containers and /var/log; adjust mounts on non-Linux hosts.
- Check container logs:
  - docker logs edinburgh_promtail
  - docker logs edinburgh_loki
  - docker logs edinburgh_grafana

## Notes
- The application still contains Datadog tracer bits in code and package references; these were not removed. If you no longer need Datadog at all, remove Datadog packages from the project file and related configuration in Program.cs.

## Resources
- Grafana: https://grafana.com/
- Loki: https://grafana.com/oss/loki/
- Promtail: https://grafana.com/oss/promtail/

