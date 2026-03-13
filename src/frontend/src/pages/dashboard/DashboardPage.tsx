import {
  Card, Row, Col, Statistic, Typography,
  Tag, Space, Spin, Empty, Table, Progress,
} from 'antd'
import {
  TeamOutlined, ProjectOutlined,
  CalendarOutlined, CheckCircleOutlined,
} from '@ant-design/icons'
import { useQuery } from '@tanstack/react-query'
import { dashboardApi } from '../../api/dashboard'
import type { RecentActivity } from '../../types'
import dayjs from 'dayjs'
import {
  BarChart, Bar, XAxis, YAxis, Tooltip,
  PieChart, Pie, Cell, ResponsiveContainer,
  Legend,
} from 'recharts'

const { Title, Text } = Typography

const PIE_COLORS = [
  '#1677ff', '#52c41a', '#fa8c16',
  '#f5222d', '#722ed1', '#13c2c2',
  '#eb2f96', '#faad14',
]

export default function DashboardPage() {
  const { data: report, isLoading } = useQuery({
    queryKey:        ['dashboard'],
    queryFn:         dashboardApi.getReport,
    refetchInterval: 60_000, // refresh every minute
  })

  if (isLoading) return (
    <div style={{ textAlign: 'center', padding: 48 }}>
      <Spin size="large" />
    </div>
  )

  if (!report) return (
    <Empty
      description="Dashboard report not yet available. The report worker generates it every 5 minutes."
    />
  )

  const { headcount, projects, leave, recentActivity } =
    report

  // Task completion percentage
  const taskCompletion = projects.totalTasks > 0
    ? Math.round(
        (projects.completedTasks /
          projects.totalTasks) * 100)
    : 0

  // Leave pie data
  const leavePieData = [
    { name: 'Pending',   value: leave.pending },
    { name: 'Approved',  value: leave.approved },
    { name: 'Rejected',  value: leave.rejected },
    { name: 'Cancelled', value: leave.cancelled },
  ].filter(d => d.value > 0)

  // Project status bar data
  const projectBarData = [
    { name: 'Active',    count: projects.active },
    { name: 'On Hold',   count: projects.onHold },
    { name: 'Completed', count: projects.completed },
    { name: 'Cancelled', count: projects.cancelled },
  ]

  // Recent activity columns
  const activityColumns = [
    {
      title:  'Event',
      key:    'event',
      render: (_: unknown, r: RecentActivity) => (
        <Space direction="vertical" size={0}>
          <Text strong style={{ fontSize: 13 }}>
            {r.eventType}
          </Text>
          <Text type="secondary" style={{ fontSize: 12 }}>
            {r.aggregateType} #{r.aggregateId}
          </Text>
        </Space>
      ),
    },
    {
      title:  'Time',
      key:    'time',
      width:  160,
      render: (_: unknown, r: RecentActivity) => (
        <Text type="secondary" style={{ fontSize: 12 }}>
          {dayjs(r.occurredAt).format('MMM D, HH:mm')}
        </Text>
      ),
    },
  ]

  return (
    <div>
      {/* Header */}
      <div className="page-header">
        <Title level={3} style={{ margin: 0 }}>
          Dashboard
        </Title>
        <Text type="secondary" style={{ fontSize: 12 }}>
          Last updated:{' '}
          {dayjs(report.generatedAt).format(
            'MMM D, YYYY HH:mm')}
        </Text>
      </div>

      {/* KPI Cards */}
      <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Total Employees"
              value={headcount.total}
              prefix={<TeamOutlined />}
              valueStyle={{ color: '#1677ff' }}
            />
            <div style={{ marginTop: 8 }}>
              <Tag color="success">
                {headcount.active} Active
              </Tag>
              <Tag color="error">
                {headcount.inactive} Inactive
              </Tag>
            </div>
          </Card>
        </Col>

        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Total Projects"
              value={projects.total}
              prefix={<ProjectOutlined />}
              valueStyle={{ color: '#52c41a' }}
            />
            <div style={{ marginTop: 8 }}>
              <Tag color="blue">
                {projects.active} Active
              </Tag>
              <Tag color="orange">
                {projects.onHold} On Hold
              </Tag>
            </div>
          </Card>
        </Col>

        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Task Completion"
              value={taskCompletion}
              suffix="%"
              prefix={<CheckCircleOutlined />}
              valueStyle={{ color: '#722ed1' }}
            />
            <Progress
              percent={taskCompletion}
              showInfo={false}
              strokeColor="#722ed1"
              style={{ marginTop: 8 }}
              size="small"
            />
          </Card>
        </Col>

        <Col xs={24} sm={12} lg={6}>
          <Card>
            <Statistic
              title="Leave Requests"
              value={leave.totalRequests}
              prefix={<CalendarOutlined />}
              valueStyle={{ color: '#fa8c16' }}
            />
            <div style={{ marginTop: 8 }}>
              <Tag color="orange">
                {leave.pending} Pending
              </Tag>
              <Tag color="success">
                {leave.approved} Approved
              </Tag>
            </div>
          </Card>
        </Col>
      </Row>

      {/* Charts Row */}
      <Row gutter={[16, 16]} style={{ marginBottom: 24 }}>
        {/* Department Headcount Bar Chart */}
        <Col xs={24} lg={14}>
          <Card title="Headcount by Department">
            <ResponsiveContainer width="100%" height={280}>
              <BarChart
                data={headcount.byDepartment}
                margin={{
                  top: 5, right: 20,
                  left: 0, bottom: 60,
                }}
              >
                <XAxis
                  dataKey="department"
                  angle={-35}
                  textAnchor="end"
                  interval={0}
                  tick={{ fontSize: 12 }}
                />
                <YAxis allowDecimals={false} />
                <Tooltip />
                <Bar
                  dataKey="count"
                  fill="#1677ff"
                  radius={[4, 4, 0, 0]}
                  name="Employees"
                />
              </BarChart>
            </ResponsiveContainer>
          </Card>
        </Col>

        {/* Leave Status Pie Chart */}
        <Col xs={24} lg={10}>
          <Card title="Leave Request Status">
            {leavePieData.length > 0 ? (
              <ResponsiveContainer
                width="100%" height={280}>
                <PieChart>
                  <Pie
                    data={leavePieData}
                    cx="50%"
                    cy="45%"
                    outerRadius={90}
                    dataKey="value"
                    label={({ name, percent }) =>
                       `${name} ${((percent ?? 0) * 100).toFixed(1)}%`}
                    labelLine={false}
                  >
                    {leavePieData.map((_, i) => (
                      <Cell
                        key={i}
                        fill={
                          PIE_COLORS[i % PIE_COLORS.length]
                        }
                      />
                    ))}
                  </Pie>
                  <Tooltip />
                  <Legend />
                </PieChart>
              </ResponsiveContainer>
            ) : (
              <Empty description="No leave data" />
            )}
          </Card>
        </Col>
      </Row>

      {/* Bottom Row */}
      <Row gutter={[16, 16]}>
        {/* Project Status */}
        <Col xs={24} lg={12}>
          <Card title="Project Status Overview">
            <ResponsiveContainer width="100%" height={200}>
              <BarChart
                data={projectBarData}
                margin={{
                  top: 5, right: 20,
                  left: 0, bottom: 5,
                }}
              >
                <XAxis
                  dataKey="name"
                  tick={{ fontSize: 12 }}
                />
                <YAxis allowDecimals={false} />
                <Tooltip />
                <Bar
                  dataKey="count"
                  radius={[4, 4, 0, 0]}
                  name="Projects"
                >
                  {projectBarData.map((entry, i) => (
                    <Cell
                      key={i}
                      fill={
                        entry.name === 'Active'
                          ? '#1677ff'
                          : entry.name === 'On Hold'
                          ? '#fa8c16'
                          : entry.name === 'Completed'
                          ? '#52c41a'
                          : '#ff4d4f'
                      }
                    />
                  ))}
                </Bar>
              </BarChart>
            </ResponsiveContainer>
          </Card>
        </Col>

        {/* Recent Activity */}
        <Col xs={24} lg={12}>
          <Card title="Recent System Activity">
            {recentActivity.length > 0 ? (
              <Table
                rowKey={(r, i) =>
                  `${r.eventType}-${i}`}
                columns={activityColumns}
                dataSource={recentActivity}
                pagination={false}
                size="small"
                scroll={{ y: 200 }}
              />
            ) : (
              <Empty
                description="No recent activity yet. Activity appears after domain events are processed by the Audit Worker."
              />
            )}
          </Card>
        </Col>
      </Row>
    </div>
  )
}