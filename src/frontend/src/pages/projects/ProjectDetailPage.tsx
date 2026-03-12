import {
  Card, Tag, Button, Typography, Space, Avatar,
  Spin, Empty, Descriptions, Select, Modal,
  Form, Input, DatePicker, message, Popconfirm,
  Row, Col,
} from 'antd'
import {
  ArrowLeftOutlined, PlusOutlined,
  UserOutlined, DeleteOutlined,
} from '@ant-design/icons'
import {
  useQuery, useMutation, useQueryClient,
} from '@tanstack/react-query'
import { useParams, useNavigate } from 'react-router-dom'
import { projectApi } from '../../api/projects'
import { taskApi }    from '../../api/tasks'
import { employeeApi } from '../../api/employees'
import type {
  Task, CreateTaskDto, AddMemberDto,
} from '../../types'
import dayjs from 'dayjs'
import { useState } from 'react'

const { Title, Text } = Typography
const { Option }      = Select
const { TextArea }    = Input

// Task status workflow columns
const TASK_STATUSES = [
  'Backlog', 'Todo', 'InProgress', 'InReview', 'Done',
]

const STATUS_COLORS: Record<string, string> = {
  Active:    'blue',
  OnHold:    'orange',
  Completed: 'green',
  Cancelled: 'red',
}

const PRIORITY_COLORS: Record<string, string> = {
  Low:      'default',
  Medium:   'blue',
  High:     'orange',
  Critical: 'red',
}

export default function ProjectDetailPage() {
  const { id }        = useParams<{ id: string }>()
  const navigate      = useNavigate()
  const queryClient   = useQueryClient()
  const projectId     = parseInt(id!)

  const [taskModal,   setTaskModal]   = useState(false)
  const [memberModal, setMemberModal] = useState(false)
  const [taskForm]    = Form.useForm()
  const [memberForm]  = Form.useForm()

  // ── Queries ───────────────────────────────────────────────
  const { data: project, isLoading } = useQuery({
    queryKey: ['project', projectId],
    queryFn:  () => projectApi.getById(projectId),
  })

  const { data: allEmployees } = useQuery({
    queryKey: ['employees', { page: 1, pageSize: 100 }],
    queryFn:  () => employeeApi.getAll({
      page: 1, pageSize: 100, isActive: true,
    }),
  })

  // ── Task Mutation ─────────────────────────────────────────
  const { mutate: createTask, isPending: creatingTask } =
    useMutation({
      mutationFn: (dto: CreateTaskDto) =>
        taskApi.create(dto),
      onSuccess: () => {
        message.success('Task created')
        queryClient.invalidateQueries({
          queryKey: ['project', projectId],
        })
        setTaskModal(false)
        taskForm.resetFields()
      },
    })

  // ── Status Update Mutation ────────────────────────────────
  const { mutate: updateTaskStatus } = useMutation({
    mutationFn: ({ taskId, status }: {
      taskId: number
      status: string
    }) => taskApi.updateStatus(taskId, status),
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ['project', projectId],
      })
    },
  })

  // ── Add Member Mutation ───────────────────────────────────
  const { mutate: addMember, isPending: addingMember } =
    useMutation({
      mutationFn: (dto: AddMemberDto) =>
        projectApi.addMember(projectId, dto),
      onSuccess: () => {
        message.success('Member added')
        queryClient.invalidateQueries({
          queryKey: ['project', projectId],
        })
        setMemberModal(false)
        memberForm.resetFields()
      },
    })

  // ── Remove Member Mutation ────────────────────────────────
  const { mutate: removeMember } = useMutation({
    mutationFn: (employeeId: number) =>
      projectApi.removeMember(projectId, employeeId),
    onSuccess: () => {
      message.success('Member removed')
      queryClient.invalidateQueries({
        queryKey: ['project', projectId],
      })
    },
  })

  if (isLoading) return (
    <div style={{ textAlign: 'center', padding: 48 }}>
      <Spin size="large" />
    </div>
  )

  if (!project) return (
    <Empty description="Project not found" />
  )

  // Group tasks by status
  const tasksByStatus = TASK_STATUSES.reduce(
    (acc, status) => {
      acc[status] = project.tasks.filter(
        t => t.status === status)
      return acc
    },
    {} as Record<string, Task[]>
  )

  // Members not yet in project
  const nonMembers = allEmployees?.items.filter(
    e => !project.members.find(
      m => m.employeeId === e.id)) || []

  return (
    <div>
      {/* Header */}
      <div className="page-header">
        <Space>
          <Button
            icon={<ArrowLeftOutlined />}
            onClick={() => navigate('/projects')}
          >
            Back
          </Button>
          <Title level={3} style={{ margin: 0 }}>
            {project.name}
          </Title>
          <Tag color={
            STATUS_COLORS[project.status] || 'default'}>
            {project.status}
          </Tag>
        </Space>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => setTaskModal(true)}
        >
          Add Task
        </Button>
      </div>

      {/* Project Info */}
      <Card style={{ marginBottom: 16 }}>
        <Descriptions size="small" column={3}>
          <Descriptions.Item label="Start Date">
            {dayjs(project.startDate)
              .format('MMM D, YYYY')}
          </Descriptions.Item>
          <Descriptions.Item label="End Date">
            {project.endDate
              ? dayjs(project.endDate)
                  .format('MMM D, YYYY')
              : 'Ongoing'}
          </Descriptions.Item>
          <Descriptions.Item label="Tasks">
            {project.completedTaskCount}/
            {project.taskCount} completed
          </Descriptions.Item>
          {project.description && (
            <Descriptions.Item
              label="Description"
              span={3}
            >
              {project.description}
            </Descriptions.Item>
          )}
        </Descriptions>
      </Card>

      {/* Team Members */}
      <Card
        title="Team Members"
        style={{ marginBottom: 16 }}
        extra={
          <Button
            size="small"
            icon={<PlusOutlined />}
            onClick={() => setMemberModal(true)}
          >
            Add Member
          </Button>
        }
      >
        <Space wrap>
          {project.members.map(member => (
            <Card
              key={member.employeeId}
              size="small"
              style={{ width: 180 }}
            >
              <Space direction="vertical"
                size={4} align="center"
                style={{ width: '100%' }}>
                <Avatar
                  src={member.avatarUrl}
                  icon={<UserOutlined />}
                  style={{ backgroundColor: '#1677ff' }}
                />
                <Text strong style={{ fontSize: 13 }}>
                  {member.fullName}
                </Text>
                <Tag color="blue" style={{ fontSize: 11 }}>
                  {member.role}
                </Tag>
                <Popconfirm
                  title="Remove this member?"
                  onConfirm={() =>
                    removeMember(member.employeeId)}
                  okText="Yes"
                  cancelText="No"
                >
                  <Button
                    type="link"
                    danger
                    size="small"
                    icon={<DeleteOutlined />}
                  >
                    Remove
                  </Button>
                </Popconfirm>
              </Space>
            </Card>
          ))}
          {project.members.length === 0 && (
            <Text type="secondary">
              No members assigned yet
            </Text>
          )}
        </Space>
      </Card>

      {/* Task Board */}
      <Card title="Task Board">
        <Row gutter={[12, 12]}>
          {TASK_STATUSES.map(status => (
            <Col
              key={status}
              xs={24} sm={12} md={8} lg={5}
            >
              {/* Column Header */}
              <div style={{
                background:   '#f5f5f5',
                borderRadius: 8,
                padding:      '8px 12px',
                marginBottom: 8,
                display:      'flex',
                justifyContent: 'space-between',
              }}>
                <Text strong style={{ fontSize: 13 }}>
                  {status === 'InProgress'
                    ? 'In Progress'
                    : status === 'InReview'
                    ? 'In Review'
                    : status}
                </Text>
                <Tag>
                  {tasksByStatus[status]?.length || 0}
                </Tag>
              </div>

              {/* Task Cards */}
              <Space
                direction="vertical"
                style={{ width: '100%' }}
              >
                {tasksByStatus[status]?.map(task => (
                  <Card
                    key={task.id}
                    size="small"
                    style={{
                      borderLeft: `3px solid ${
                        PRIORITY_COLORS[task.priority] ===
                          'red' ? '#ff4d4f' :
                        PRIORITY_COLORS[task.priority] ===
                          'orange' ? '#fa8c16' :
                        PRIORITY_COLORS[task.priority] ===
                          'blue' ? '#1677ff' : '#d9d9d9'
                      }`,
                    }}
                  >
                    <Text strong style={{ fontSize: 13 }}>
                      {task.title}
                    </Text>

                    {task.description && (
                      <Text
                        type="secondary"
                        style={{
                          fontSize: 12,
                          display:  'block',
                          marginTop: 4,
                        }}
                      >
                        {task.description
                          .substring(0, 60)}
                        {task.description.length > 60
                          ? '...' : ''}
                      </Text>
                    )}

                    <div style={{ marginTop: 8 }}>
                      <Tag
                        color={
                          PRIORITY_COLORS[task.priority]}
                        style={{ fontSize: 11 }}
                      >
                        {task.priority}
                      </Tag>
                      {task.assignedToName && (
                        <Tag
                          icon={<UserOutlined />}
                          style={{ fontSize: 11 }}
                        >
                          {task.assignedToName
                            .split(' ')[0]}
                        </Tag>
                      )}
                    </div>

                    {task.dueDate && (
                      <Text
                        type="secondary"
                        style={{
                          fontSize: 11,
                          display:  'block',
                          marginTop: 4,
                        }}
                      >
                        Due:{' '}
                        {dayjs(task.dueDate)
                          .format('MMM D')}
                      </Text>
                    )}

                    {/* Move Task */}
                    <Select
                      size="small"
                      value={task.status}
                      style={{
                        width:     '100%',
                        marginTop: 8,
                        fontSize:  11,
                      }}
                      onChange={val =>
                        updateTaskStatus({
                          taskId: task.id,
                          status: val,
                        })
                      }
                    >
                      {TASK_STATUSES.map(s => (
                        <Option key={s} value={s}>
                          {s === 'InProgress'
                            ? 'In Progress'
                            : s === 'InReview'
                            ? 'In Review'
                            : s}
                        </Option>
                      ))}
                    </Select>
                  </Card>
                ))}

                {tasksByStatus[status]?.length === 0 && (
                  <div style={{
                    border:       '2px dashed #f0f0f0',
                    borderRadius: 8,
                    padding:      16,
                    textAlign:    'center',
                  }}>
                    <Text
                      type="secondary"
                      style={{ fontSize: 12 }}
                    >
                      No tasks
                    </Text>
                  </div>
                )}
              </Space>
            </Col>
          ))}
        </Row>
      </Card>

      {/* Create Task Modal */}
      <Modal
        title="Add Task"
        open={taskModal}
        onCancel={() => {
          setTaskModal(false)
          taskForm.resetFields()
        }}
        footer={null}
        width={520}
      >
        <Form
          form={taskForm}
          layout="vertical"
          initialValues={{
            status:   'Backlog',
            priority: 'Medium',
          }}
          onFinish={values => {
            createTask({
              title:       values.title,
              description: values.description,
              status:      values.status,
              priority:    values.priority,
              dueDate:     values.dueDate
                ? dayjs(values.dueDate)
                    .format('YYYY-MM-DD')
                : undefined,
              projectId,
              assignedToId: values.assignedToId,
            })
          }}
        >
          <Form.Item
            name="title"
            label="Title"
            rules={[{ required: true }]}
          >
            <Input placeholder="Task title" />
          </Form.Item>

          <Form.Item
            name="description"
            label="Description"
          >
            <TextArea rows={2} />
          </Form.Item>

          <Form.Item name="status" label="Status">
            <Select>
              {TASK_STATUSES.map(s => (
                <Option key={s} value={s}>
                  {s === 'InProgress' ? 'In Progress'
                    : s === 'InReview' ? 'In Review'
                    : s}
                </Option>
              ))}
            </Select>
          </Form.Item>

          <Form.Item name="priority" label="Priority">
            <Select>
              <Option value="Low">Low</Option>
              <Option value="Medium">Medium</Option>
              <Option value="High">High</Option>
              <Option value="Critical">Critical</Option>
            </Select>
          </Form.Item>

          <Form.Item
            name="assignedToId"
            label="Assign To"
          >
            <Select
              allowClear
              placeholder="Select team member"
            >
              {project.members.map(m => (
                <Option
                  key={m.employeeId}
                  value={m.employeeId}
                >
                  {m.fullName}
                </Option>
              ))}
            </Select>
          </Form.Item>

          <Form.Item name="dueDate" label="Due Date">
            <DatePicker style={{ width: '100%' }} />
          </Form.Item>

          <Form.Item style={{ marginBottom: 0 }}>
            <Space style={{ float: 'right' }}>
              <Button onClick={() => {
                setTaskModal(false)
                taskForm.resetFields()
              }}>
                Cancel
              </Button>
              <Button
                type="primary"
                htmlType="submit"
                loading={creatingTask}
              >
                Create Task
              </Button>
            </Space>
          </Form.Item>
        </Form>
      </Modal>

      {/* Add Member Modal */}
      <Modal
        title="Add Team Member"
        open={memberModal}
        onCancel={() => {
          setMemberModal(false)
          memberForm.resetFields()
        }}
        footer={null}
        width={400}
      >
        <Form
          form={memberForm}
          layout="vertical"
          initialValues={{ role: 'Member' }}
          onFinish={values => {
            addMember({
              employeeId: values.employeeId,
              role:       values.role,
            })
          }}
        >
          <Form.Item
            name="employeeId"
            label="Employee"
            rules={[{ required: true }]}
          >
            <Select
              showSearch
              placeholder="Select employee"
              optionFilterProp="children"
            >
              {nonMembers.map(e => (
                <Option key={e.id} value={e.id}>
                  {e.fullName} — {e.departmentName}
                </Option>
              ))}
            </Select>
          </Form.Item>

          <Form.Item
            name="role"
            label="Role"
            rules={[{ required: true }]}
          >
            <Select>
              <Option value="Lead">Lead</Option>
              <Option value="Member">Member</Option>
              <Option value="Reviewer">Reviewer</Option>
            </Select>
          </Form.Item>

          <Form.Item style={{ marginBottom: 0 }}>
            <Space style={{ float: 'right' }}>
              <Button onClick={() => {
                setMemberModal(false)
                memberForm.resetFields()
              }}>
                Cancel
              </Button>
              <Button
                type="primary"
                htmlType="submit"
                loading={addingMember}
              >
                Add Member
              </Button>
            </Space>
          </Form.Item>
        </Form>
      </Modal>
    </div>
  )
}