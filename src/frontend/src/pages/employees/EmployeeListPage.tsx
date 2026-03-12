import { useState } from 'react'
import {
  Table, Button, Input, Select, Space,
  Tag, Avatar, Typography, Row, Col,
  Card, Statistic,
} from 'antd'
import {
  PlusOutlined, SearchOutlined,
  UserOutlined, TeamOutlined,
} from '@ant-design/icons'
import { useQuery } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { employeeApi } from '../../api/employees'
import { departmentApi } from '../../api/departments'
import type { Employee, EmployeeQueryParams } from '../../types'
import dayjs from 'dayjs'

const { Title } = Typography
const { Option } = Select

export default function EmployeeListPage() {
  const navigate = useNavigate()

  const [params, setParams] = useState<EmployeeQueryParams>({
    page:     1,
    pageSize: 10,
  })

  // ── Queries ───────────────────────────────────────────────
  const { data, isLoading } = useQuery({
    queryKey: ['employees', params],
    queryFn:  () => employeeApi.getAll(params),
  })

  const { data: departments } = useQuery({
    queryKey: ['departments'],
    queryFn:  departmentApi.getAll,
  })

  // ── Columns ───────────────────────────────────────────────
  const columns = [
    {
      title:  'Employee',
      key:    'employee',
      render: (_: unknown, record: Employee) => (
        <Space>
          <Avatar
            src={record.avatarUrl}
            icon={<UserOutlined />}
            style={{ backgroundColor: '#1677ff' }}
          />
          <Space direction="vertical" size={0}>
            <Typography.Text strong>
              {record.fullName}
            </Typography.Text>
            <Typography.Text
              type="secondary"
              style={{ fontSize: 12 }}
            >
              {record.email}
            </Typography.Text>
          </Space>
        </Space>
      ),
    },
    {
      title:     'Department',
      dataIndex: 'departmentName',
      key:       'departmentName',
    },
    {
      title:     'Designation',
      dataIndex: 'designationName',
      key:       'designationName',
    },
    {
      title:  'Skills',
      key:    'skills',
      render: (_: unknown, record: Employee) => (
        <Space size={[0, 4]} wrap>
          {record.skills.slice(0, 3).map(skill => (
            <Tag key={skill} color="blue">
              {skill}
            </Tag>
          ))}
          {record.skills.length > 3 && (
            <Tag>+{record.skills.length - 3}</Tag>
          )}
        </Space>
      ),
    },
    {
      title:  'Joining Date',
      key:    'joiningDate',
      render: (_: unknown, record: Employee) =>
        dayjs(record.joiningDate).format('MMM D, YYYY'),
    },
    {
      title:  'Status',
      key:    'isActive',
      render: (_: unknown, record: Employee) => (
        <Tag color={record.isActive ? 'success' : 'error'}>
          {record.isActive ? 'Active' : 'Inactive'}
        </Tag>
      ),
    },
    {
      title:  'Action',
      key:    'action',
      render: (_: unknown, record: Employee) => (
        <Button
          type="link"
          onClick={e => {
            e.stopPropagation()
            navigate(`/employees/${record.id}`)
          }}
        >
          View
        </Button>
      ),
    },
  ]

  // ── Active count from current page ───────────────────────
  const activeCount = data?.items.filter(
    e => e.isActive).length ?? 0

  return (
    <div>
      {/* Header */}
      <div className="page-header">
        <Title level={3} style={{ margin: 0 }}>
          Employees
        </Title>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => navigate('/employees/new')}
        >
          Add Employee
        </Button>
      </div>

      {/* Stats Cards */}
      <Row gutter={[16, 16]} style={{ marginBottom: 16 }}>
        <Col xs={24} sm={8}>
          <Card>
            <Statistic
              title="Total Employees"
              value={data?.totalCount ?? 0}
              prefix={<TeamOutlined />}
              valueStyle={{ color: '#1677ff' }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={8}>
          <Card>
            <Statistic
              title="Active (this page)"
              value={activeCount}
              valueStyle={{ color: '#52c41a' }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={8}>
          <Card>
            <Statistic
              title="Departments"
              value={departments?.length ?? 0}
              valueStyle={{ color: '#722ed1' }}
            />
          </Card>
        </Col>
      </Row>

      {/* Filters */}
      <Card style={{ marginBottom: 16 }}>
        <Row gutter={16}>
          <Col xs={24} sm={8}>
            <Input
              placeholder="Search by name or email..."
              prefix={<SearchOutlined />}
              allowClear
              onChange={e => setParams(p => ({
                ...p,
                page:   1,
                search: e.target.value || undefined,
              }))}
            />
          </Col>
          <Col xs={24} sm={6}>
            <Select
              placeholder="Filter by Department"
              allowClear
              style={{ width: '100%' }}
              onChange={val => setParams(p => ({
                ...p,
                page:         1,
                departmentId: val,
              }))}
            >
              {departments?.map(d => (
                <Option key={d.id} value={d.id}>
                  {d.name}
                </Option>
              ))}
            </Select>
          </Col>
          <Col xs={24} sm={6}>
            <Select
              placeholder="Filter by Status"
              allowClear
              style={{ width: '100%' }}
              onChange={val => setParams(p => ({
                ...p,
                page:     1,
                isActive: val,
              }))}
            >
              <Option value={true}>Active</Option>
              <Option value={false}>Inactive</Option>
            </Select>
          </Col>
        </Row>
      </Card>

      {/* Table */}
      <Table
        rowKey="id"
        columns={columns} 
        dataSource={data?.items}
        loading={isLoading}
        pagination={{
          current:        params.page,
          pageSize:       params.pageSize,
          total:          data?.totalCount,
          showSizeChanger: true,
          showTotal:      total =>
            `Total ${total} employees`,
          onChange: (page, pageSize) =>
            setParams(p => ({ ...p, page, pageSize })),
        }}
        onRow={record => ({
          onClick: () =>
            navigate(`/employees/${record.id}`),
          style: { cursor: 'pointer' },
        })}
      />
    </div>
  )
}